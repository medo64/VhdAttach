//Copyright (c) 2011 Josip Medved <jmedved@jmedved.com>

//2011-08-26: Initial version (based on TinyMessage).
//2011-10-22: Adjusted to work on Mono.
//            Added IsListening property.
//2011-10-24: Added UseObjectEncoding.
//2011-11-07: Fixing encoding/decoding.
//            Changed all parsing errors to throw FormatException.
//2012-02-06: Renamed to TinyMessage.
//            Removed array encoding.
//            TinyPacket data items are accessible through indexed property.
//            Null strings can be encoded.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Medo.Net {

    /// <summary>
    /// Sending and receiving UDP messages.
    /// Supports TinyMessage with Dictionary&lt;string,string&gt; as data.
    /// </summary>
    public class TinyMessage : IDisposable {

        /// <summary>
        /// Default port for TinyMessage protocol.
        /// </summary>
        public static int DefaultPort { get { return 5104; } }


        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyMessage()
            : this(new IPEndPoint(IPAddress.Any, TinyMessage.DefaultPort)) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="localEndPoint">Local end point where messages should be received at.</param>
        /// <exception cref="System.ArgumentNullException">Local IP end point is null.</exception>
        public TinyMessage(IPEndPoint localEndPoint) {
            if (localEndPoint == null) { throw new ArgumentNullException("localEndPoint", "Local IP end point is null."); }
            this.LocalEndPoint = localEndPoint;
        }

        /// <summary>
        /// Gets local IP end point.
        /// </summary>
        public IPEndPoint LocalEndPoint { get; private set; }


        /// <summary>
        /// Starts listener on background thread.
        /// </summary>
        public void ListenAsync() {
            lock (this.ListenSyncRoot) {
                if (this.ListenThread != null) { throw new InvalidOperationException("Already listening."); }

                this.ListenCancelEvent = new ManualResetEvent(false);
                this.ListenThread = new Thread(Run) { IsBackground = true, Name = "TinyMessage " + this.LocalEndPoint.ToString() };
                this.ListenThread.Start();
            }
        }

        /// <summary>
        /// Stops listener on background thread.
        /// </summary>
        public void CloseAsync() {
            lock (this.ListenSyncRoot) {
                if (this.ListenThread == null) { return; }

                this.ListenCancelEvent.Set();
                this.ListenSocket.Shutdown(SocketShutdown.Both);
                this.ListenSocket.Close();

                while (this.ListenThread.IsAlive) { Thread.Sleep(100); }
                ((IDisposable)this.ListenCancelEvent).Dispose();
                this.ListenThread = null;
            }
        }

        /// <summary>
        /// Gets whether TinyMessage is in listening state.
        /// </summary>
        public bool IsListening {
            get { return (this.ListenThread != null) && (this.ListenThread.IsAlive); }
        }

        /// <summary>
        /// Raises event when packet arrives.
        /// </summary>
        public event EventHandler<TinyPacketEventArgs> TinyPacketReceived;


        #region Threading

        private Thread ListenThread;
        private ManualResetEvent ListenCancelEvent = null;
        private readonly object ListenSyncRoot = new object();
        private Socket ListenSocket = null;

        private bool IsCanceled { get { return ListenCancelEvent.WaitOne(0, false); } }

        private void Run() {
            try {
                this.ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.ListenSocket.Bind(this.LocalEndPoint);

                var buffer = new byte[16384];
                EndPoint remoteEP;
                int inCount;
                while (!this.IsCanceled) {
                    try {
                        remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        inCount = this.ListenSocket.ReceiveFrom(buffer, ref remoteEP);
                    } catch (SocketException ex) {
                        if (ex.SocketErrorCode == SocketError.Interrupted) {
                            return;
                        } else {
                            throw;
                        }
                    }

                    if (TinyPacketReceived != null) {
                        var newBuffer = new byte[inCount];
                        Buffer.BlockCopy(buffer, 0, newBuffer, 0, inCount);
                        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyMessage [{0} <- {1}]", TinyPacket.ParseHeaderOnly(newBuffer, 0, inCount), remoteEP));
                        var invokeArgs = new object[] { this, new TinyPacketEventArgs(newBuffer, 0, inCount, remoteEP as IPEndPoint) };
                        foreach (Delegate iDelegate in TinyPacketReceived.GetInvocationList()) {
                            ISynchronizeInvoke syncer = iDelegate.Target as ISynchronizeInvoke;
                            if (syncer == null) {
                                iDelegate.DynamicInvoke(invokeArgs);
                            } else {
                                syncer.BeginInvoke(iDelegate, invokeArgs);
                            }
                        }
                    }
                }

            } catch (ThreadAbortException) {
            } finally {
                if (this.ListenSocket != null) {
                    ((IDisposable)this.ListenSocket).Dispose();
                    this.ListenSocket = null;
                }
            }

        }

        #endregion


        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static void Send(TinyPacket packet, IPAddress address) {
            Send(packet, new IPEndPoint(address, TinyMessage.DefaultPort));
        }

        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <param name="port">Port of destination for packet.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static void Send(TinyPacket packet, IPAddress address, int port) {
            Send(packet, new IPEndPoint(address, port));
        }

        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="remoteEndPoint">Address of destination for packet. It can be broadcast address.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static void Send(TinyPacket packet, IPEndPoint remoteEndPoint) {
            if (packet == null) { throw new ArgumentNullException("packet", "Packet is null."); }
            if (remoteEndPoint == null) { throw new ArgumentNullException("remoteEndPoint", "Remote IP end point is null."); }
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                if (remoteEndPoint.Address == IPAddress.Broadcast) {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                }
                if (IsRunningOnMono == false) { socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoChecksum, false); }
                socket.SendTo(packet.GetBytes(), remoteEndPoint);
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyMessage [{0} -> {1}]", packet, remoteEndPoint));
            }
        }


        /// <summary>
        /// Sends one UDP packet and expects another in return.
        /// Returns null if respose is not received within 250 milliseconds.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static TinyPacket SendAndReceive(TinyPacket packet, IPAddress address) {
            return SendAndReceive(packet, new IPEndPoint(address, TinyMessage.DefaultPort), 250);
        }

        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// Sends one UDP packet and expects another in return.
        /// Returns null if respose is not received within 250 milliseconds.
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <param name="port">Port of destination for packet.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static TinyPacket SendAndReceive(TinyPacket packet, IPAddress address, int port) {
            return SendAndReceive(packet, new IPEndPoint(address, port), 250);
        }

        /// <summary>
        /// Sends one UDP packet and expects another in return.
        /// Returns null if respose is not received within receiveTimeout.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="remoteEndPoint">Address of destination for packet. It can be broadcast address.</param>
        /// <param name="receiveTimeout">Number of milliseconds to wait for receive operation to be done. If number is zero, infinite timeout will be used.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static TinyPacket SendAndReceive(TinyPacket packet, IPEndPoint remoteEndPoint, int receiveTimeout) {
            if (packet == null) { throw new ArgumentNullException("packet", "Packet is null."); }
            if (remoteEndPoint == null) { throw new ArgumentNullException("remoteEndPoint", "Remote IP end point is null."); }
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                socket.ReceiveTimeout = receiveTimeout;
                if (remoteEndPoint.Address == IPAddress.Broadcast) {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                }
                if (IsRunningOnMono == false) { socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoChecksum, false); }
                var bytesOut = packet.GetBytes();
                socket.SendTo(bytesOut, bytesOut.Length, SocketFlags.None, remoteEndPoint);
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyMessage [{0} -> {1}]", packet, remoteEndPoint));

                EndPoint remoteEndPointIn = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                var bytesIn = new byte[65536];
                try {
                    int len = socket.ReceiveFrom(bytesIn, ref remoteEndPointIn);
                    var packetIn = Medo.Net.TinyPacket.Parse(bytesIn, 0, len);
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyMessage [{0} <- {1}]", packetIn, remoteEndPointIn));
                    return packetIn;
                } catch (SocketException) {
                    return null;
                }
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.CloseAsync();
            }
        }

        #endregion

        private static bool IsRunningOnMono { get { return (Type.GetType("Mono.Runtime") != null); } }

    }



    /// <summary>
    /// Encoder/decoder for Tiny packets.
    /// </summary>
    public class TinyPacket : IDisposable {

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="product">Name of product. Preferred format would be application name, at (@) sign, IANA assigned Private Enterprise Number. E.g. Application@12345</param>
        /// <param name="operation">Message type.</param>
        /// <exception cref="System.ArgumentNullException">Product is null or empty. -or- Operation is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Product contains space character. -or- Operation contains space character.</exception>
        public TinyPacket(string product, string operation) {
            if (string.IsNullOrEmpty(product)) { throw new ArgumentNullException("product", "Product is null or empty."); }
            if (product.Contains(" ")) { throw new ArgumentException("Product contains space character.", "product"); }
            if (string.IsNullOrEmpty(operation)) { throw new ArgumentNullException("operation", "Operation is null or empty."); }
            if (operation.Contains(" ")) { throw new ArgumentException("Operation contains space character.", "operation"); }

            this.Product = product;
            this.Operation = operation;
            this.Items = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="product">Name of product. Preferred format would be application name, at (@) sign, IANA assigned Private Enterprise Number. E.g. Application@12345</param>
        /// <param name="operation">Message type.</param>
        /// <param name="items">Data to be encoded in JSON.</param>
        /// <exception cref="System.ArgumentNullException">Product is null or empty. -or- Operation is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Product contains space character. -or- Operation contains space character.</exception>
        internal TinyPacket(string product, string operation, IDictionary<string, string> items) {
            if (string.IsNullOrEmpty(product)) { throw new ArgumentNullException("product", "Product is null or empty."); }
            if (product.Contains(" ")) { throw new ArgumentException("Product contains space character.", "product"); }
            if (string.IsNullOrEmpty(operation)) { throw new ArgumentNullException("operation", "Operation is null or empty."); }
            if (operation.Contains(" ")) { throw new ArgumentException("Operation contains space character.", "operation"); }

            this.Product = product;
            this.Operation = operation;
            this.Items = items;
            this.IsReadOnly = true;
        }

        /// <summary>
        /// Gets name of product.
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Gets operation.
        /// </summary>
        public string Operation { get; private set; }

        private readonly bool IsReadOnly;
        private readonly IDictionary<string, string> Items;

        /// <summary>
        /// Gets/sets data item.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <exception cref="System.NotSupportedException">Data is read-only.</exception>
        public string this[string key] {
            get {
                if (this.Items.ContainsKey(key)) {
                    return this.Items[key];
                } else {
                    return null;
                }
            }
            set {
                if (this.IsReadOnly) { throw new NotSupportedException("Data is read-only."); }
                if (this.Items.ContainsKey(key)) {
                    this.Items[key] = value;
                } else {
                    this.Items.Add(key, value);
                }
            }
        }


        private static readonly UTF8Encoding TextEncoding = new UTF8Encoding(false);

        /// <summary>
        /// Converts message to it's representation in bytes.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Packet length exceeds 65507 bytes.</exception>
        public byte[] GetBytes() {
            using (var stream = new MemoryStream()) {
                byte[] protocolBytes = TextEncoding.GetBytes("Tiny");
                stream.Write(protocolBytes, 0, protocolBytes.Length);
                stream.Write(new byte[] { 0x20 }, 0, 1);

                byte[] productBytes = TextEncoding.GetBytes(this.Product);
                stream.Write(productBytes, 0, productBytes.Length);
                stream.Write(new byte[] { 0x20 }, 0, 1);

                byte[] operationBytes = TextEncoding.GetBytes(this.Operation);
                stream.Write(operationBytes, 0, operationBytes.Length);
                stream.Write(new byte[] { 0x20 }, 0, 1);

                var addComma = false;
                if (this.Items != null) {
                    stream.Write(new byte[] { 0x7B }, 0, 1); //{
                    foreach (var item in this.Items) {
                        if (addComma) { stream.Write(new byte[] { 0x2C }, 0, 1); } //,
                        byte[] keyBytes = TextEncoding.GetBytes(JsonEncode(item.Key));
                        stream.Write(new byte[] { 0x22 }, 0, 1); //"
                        stream.Write(keyBytes, 0, keyBytes.Length);
                        stream.Write(new byte[] { 0x22, 0x3A }, 0, 2); //":
                        if (item.Value != null) {
                            byte[] valueBytes = TextEncoding.GetBytes(JsonEncode(item.Value));
                            stream.Write(new byte[] { 0x22 }, 0, 1); //"
                            stream.Write(valueBytes, 0, valueBytes.Length);
                            stream.Write(new byte[] { 0x22 }, 0, 1); //"
                        } else {
                            stream.Write(new byte[] { 0x6E, 0x75, 0x6C, 0x6C }, 0, 4); //null
                        }
                        addComma = true;
                    }
                    stream.Write(new byte[] { 0x7D }, 0, 1); //}
                } else {
                    stream.Write(new byte[] { 0x6E, 0x75, 0x6C, 0x6C }, 0, 4); //null
                }

                if (stream.Position > 65507) { throw new InvalidOperationException("Packet length exceeds 65507 bytes."); }

                return stream.ToArray();
            }
        }

        private static string JsonEncode(string value) {
            var sb = new StringBuilder();
            foreach (char ch in value) {
                switch (ch) {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '/': sb.Append(@"\/"); break;
                    case '\b': sb.Append(@"\b"); break;
                    case '\f': sb.Append(@"\f"); break;
                    case '\n': sb.Append(@"\n"); break;
                    case '\r': sb.Append(@"\r"); break;
                    case '\t': sb.Append(@"\t"); break;
                    default:
                        if (char.IsControl(ch)) {
                            sb.Append("\\u" + ((int)ch).ToString("x4", CultureInfo.InvariantCulture));
                        } else {
                            sb.Append(ch);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns parsed packet.
        /// </summary>
        /// <param name="buffer">Byte array.</param>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">Cannot parse packet.</exception>
        public static TinyPacket Parse(byte[] buffer) {
            if (buffer == null) { throw new ArgumentNullException("buffer", "Buffer is null."); }

            return Parse(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Returns partially parsed packet. Data argument is NOT parsed.
        /// </summary>
        /// <param name="buffer">Byte array.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="count">Total lenght.</param>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Offset is less than zero. -or- Count is less than zero. -or- The sum of offset and count is greater than the length of buffer.</exception>
        /// <exception cref="System.FormatException">Cannot parse packet.</exception>
        public static TinyPacket ParseHeaderOnly(byte[] buffer, int offset, int count) {
            if (buffer == null) { throw new ArgumentNullException("buffer", "Buffer is null."); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset", "Index is less than zero."); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", "Count is less than zero."); }
            if (offset + count > buffer.Length) { throw new ArgumentOutOfRangeException("count", "The sum of offset and count is greater than the length of buffer."); }

            using (var stream = new MemoryStream(buffer, offset, count)) {
                string protocol = ReadToSpaceOrEnd(stream);
                if (string.Equals(protocol, "Tiny", StringComparison.Ordinal) == false) { throw new System.FormatException("Cannot parse packet."); }

                string product = ReadToSpaceOrEnd(stream);
                if (string.IsNullOrEmpty(product)) { throw new System.FormatException("Cannot parse packet."); }
                string operation = ReadToSpaceOrEnd(stream);
                if (string.IsNullOrEmpty(operation)) { throw new System.FormatException("Cannot parse packet."); }

                return new TinyPacket(product, operation, null);
            }
        }

        /// <summary>
        /// Returns parsed packet.
        /// </summary>
        /// <param name="buffer">Byte array.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="count">Total lenght.</param>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Offset is less than zero. -or- Count is less than zero. -or- The sum of offset and count is greater than the length of buffer.</exception>
        /// <exception cref="System.FormatException">Cannot parse packet.</exception>
        public static TinyPacket Parse(byte[] buffer, int offset, int count) {
            if (buffer == null) { throw new ArgumentNullException("buffer", "Buffer is null."); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset", "Index is less than zero."); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", "Count is less than zero."); }
            if (offset + count > buffer.Length) { throw new ArgumentOutOfRangeException("count", "The sum of offset and count is greater than the length of buffer."); }

            using (var stream = new MemoryStream(buffer, offset, count)) {
                string protocol = ReadToSpaceOrEnd(stream);
                if (string.Equals(protocol, "Tiny", StringComparison.Ordinal) == false) { throw new System.FormatException("Cannot parse packet."); }

                string product = ReadToSpaceOrEnd(stream);
                if (string.IsNullOrEmpty(product)) { throw new System.FormatException("Cannot parse packet."); }
                string operation = ReadToSpaceOrEnd(stream);
                if (string.IsNullOrEmpty(operation)) { throw new System.FormatException("Cannot parse packet."); }

                var data = new Dictionary<string, string>();

                var jsonBytes = new byte[stream.Length - stream.Position];
                if (jsonBytes.Length > 0) {
                    stream.Read(jsonBytes, 0, jsonBytes.Length);
                    var jsonText = new Queue<char>(TextEncoding.GetString(jsonBytes));
                    while (true) { //remove whitespace and determine content kind
                        var ch = jsonText.Peek();
                        if (ch == ' ') {
                            jsonText.Dequeue();
                        } else if (ch == '{') {
                            ParseJsonObject(jsonText, data);
                            break;
                        } else {
                            if ((jsonText.Count > 0) && (jsonText.Dequeue() == 'n')) {
                                if ((jsonText.Count > 0) && (jsonText.Dequeue() == 'u')) {
                                    if ((jsonText.Count > 0) && (jsonText.Dequeue() == 'l')) {
                                        if ((jsonText.Count > 0) && (jsonText.Dequeue() == 'l')) {
                                            while ((jsonText.Count > 0) && (jsonText.Peek() == ' ')) {
                                                jsonText.Dequeue();
                                            }
                                            if (jsonText.Count == 0) { break; }
                                        }
                                    }
                                }
                            }
                            throw new System.FormatException("Cannot determine data kind.");
                        }
                    }
                }
                return new TinyPacket(product, operation, data);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cyclomatic complexity is actually lower than code analysis shows.")]
        private static JsonState ParseJsonObject(Queue<char> jsonText, Dictionary<string, string> data) {
            var state = JsonState.Default;
            var sbName = new StringBuilder();
            var sbValue = new StringBuilder();
            while (jsonText.Count > 0) {
                var ch = jsonText.Dequeue();
                switch (state) {
                    case JsonState.Default: {
                            switch (ch) {
                                case '{': state = JsonState.LookingForNameStart; break;
                                default: throw new System.FormatException("Cannot find item start.");
                            }
                        } break;

                    case JsonState.LookingForNameStart: {
                            switch (ch) {
                                case ' ': break;
                                case '}': state = JsonState.DeadEnd; break; //empty object
                                case '\"': state = JsonState.LookingForNameEnd; break;
                                default: throw new System.FormatException("Cannot find key name start.");
                            }
                        } break;

                    case JsonState.LookingForNameEnd: {
                            switch (ch) {
                                case '\\': sbName.Append(Descape(jsonText)); break;
                                case '\"': state = JsonState.LookingForPairSeparator; break;
                                default: sbName.Append(ch); break;
                            }
                        } break;

                    case JsonState.LookingForPairSeparator: {
                            switch (ch) {
                                case ' ': break;
                                case ':': state = JsonState.LookingForValueStart; break;
                                default: throw new System.FormatException("Cannot find name/value separator.");
                            }
                        } break;

                    case JsonState.LookingForValueStart: {
                            switch (ch) {
                                case ' ': break;
                                case '\"': state = JsonState.LookingForValueEnd; break;
                                case 'n': state = JsonState.LookingForNullChar2; break;
                                default: throw new System.FormatException("Cannot find key value start.");
                            }
                        } break;

                    case JsonState.LookingForValueEnd: {
                            switch (ch) {
                                case '\\': sbValue.Append(Descape(jsonText)); break;
                                case '\"':
                                    var name = sbName.ToString();
                                    var value = sbValue.ToString();
                                    if (data.ContainsKey(name)) {
                                        data[name] = value;
                                    } else {
                                        data.Add(name, value);
                                    }
                                    sbName.Length = 0;
                                    sbValue.Length = 0;
                                    state = JsonState.LookingForObjectEnd;
                                    break;
                                default: sbValue.Append(ch); break;
                            }
                        } break;

                    case JsonState.LookingForNullChar2: {
                            switch (ch) {
                                case 'u': state = JsonState.LookingForNullChar3; break;
                                default: throw new System.FormatException("Cannot find null.");
                            }
                        } break;

                    case JsonState.LookingForNullChar3: {
                            switch (ch) {
                                case 'l': state = JsonState.LookingForNullChar4; break;
                                default: throw new System.FormatException("Cannot find null.");
                            }
                        } break;

                    case JsonState.LookingForNullChar4: {
                            switch (ch) {
                                case 'l':
                                    var name = sbName.ToString();
                                    if (data.ContainsKey(name)) {
                                        data[name] = null;
                                    } else {
                                        data.Add(name, null);
                                    }
                                    sbName.Length = 0;
                                    sbValue.Length = 0;
                                    state = JsonState.LookingForObjectEnd;
                                    break;
                                default: throw new System.FormatException("Cannot find null.");
                            }
                        } break;

                    case JsonState.LookingForObjectEnd: {
                            switch (ch) {
                                case ' ': break;
                                case ',': state = JsonState.LookingForNameStart; break;
                                case '}': state = JsonState.DeadEnd; break;
                                default: throw new System.FormatException("Cannot find item start.");
                            }
                        } break;

                    case JsonState.DeadEnd: {
                            switch (ch) {
                                case ' ': break;
                                default: throw new System.FormatException("Unexpected data.");
                            }
                        } break;

                    default: throw new System.FormatException("Unexpected state.");

                }
            }
            return state;
        }

        private static string Descape(Queue<char> jsonText) {
            var ch = jsonText.Dequeue();
            switch (ch) {
                case '\"': return "\"";
                case '\\': return "\\";
                case '/': return "/";
                case 'b': return System.Convert.ToChar(0x08).ToString();
                case 'f': return System.Convert.ToChar(0x0C).ToString();
                case 'n': return System.Convert.ToChar(0x0A).ToString();
                case 'r': return System.Convert.ToChar(0x0D).ToString();
                case 't': return System.Convert.ToChar(0x09).ToString();
                case 'u':
                    var hex = new string(new char[] { jsonText.Dequeue(), jsonText.Dequeue(), jsonText.Dequeue(), jsonText.Dequeue() });
                    var codepoint = UInt32.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return System.Convert.ToChar(codepoint).ToString();
                default: throw new System.FormatException("Cannot decode escape sequence.");
            }
        }

        private static string ReadToSpaceOrEnd(MemoryStream stream) {
            var bytes = new List<byte>(); ;
            while (stream.Position < stream.Length) {
                var oneByte = (byte)stream.ReadByte();
                if (oneByte == 0x20) {
                    break;
                } else {
                    bytes.Add(oneByte);
                }
            }
            return TextEncoding.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode() {
            return (this.Product).GetHashCode() ^ this.Operation.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() {
            return this.Product + ":" + this.Operation;
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.Product = null;
                this.Operation = null;
                this.Items.Clear();
            }
        }

        #endregion


        private enum JsonState {
            Default,
            LookingForObjectStart,
            LookingForNameStart,
            LookingForNameEnd,
            LookingForPairSeparator,
            LookingForValueStart,
            LookingForValueEnd,
            LookingForObjectEnd,
            LookingForObjectSeparator,
            LookingForNullChar2,
            LookingForNullChar3,
            LookingForNullChar4,
            DeadEnd,
        }

    }



    /// <summary>
    /// Event arguments for TinyPacketReceived message.
    /// </summary>
    public class TinyPacketEventArgs : EventArgs {

        private readonly byte[] Buffer;
        private readonly int Offset;
        private readonly int Count;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count.</param>
        /// <param name="remoteEndPoint">Remote end point.</param>
        /// <exception cref="System.ArgumentNullException">Buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Offset is less than zero. -or- Count is less than zero. -or- The sum of offset and count is greater than the length of buffer.</exception>
        public TinyPacketEventArgs(byte[] buffer, int offset, int count, IPEndPoint remoteEndPoint) {
            if (buffer == null) { throw new ArgumentNullException("buffer", "Buffer is null."); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset", "Index is less than zero."); }
            if (count < 0) { throw new ArgumentOutOfRangeException("count", "Count is less than zero."); }
            if (offset + count > buffer.Length) { throw new ArgumentOutOfRangeException("count", "The sum of offset and count is greater than the length of buffer."); }

            this.Buffer = buffer;
            this.Offset = offset;
            this.Count = count;
            this.RemoteEndPoint = remoteEndPoint;
        }

        /// <summary>
        /// Gets end point that was origin of message.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// Returns parsed packet.
        /// </summary>
        /// <exception cref="System.FormatException">Cannot parse packet.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method might throw exception.")]
        public TinyPacket GetPacket() {
            return TinyPacket.Parse(this.Buffer, this.Offset, this.Count);
        }

        /// <summary>
        /// Returns parsed packet.
        /// </summary>
        /// <exception cref="System.FormatException">Cannot parse packet.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is appropriate here.")]
        public TinyPacket GetPacketWithoutData() {
            return TinyPacket.ParseHeaderOnly(this.Buffer, this.Offset, this.Count);
        }

    }
}



/*

                          TinyMessage protocol                          

TinyMessage is text based protocol. Each packet is encupselated in UDP
datagram and it is of following content (each part encoded as UTF8,
without quotes ("), <SP> denotes space):
"Protocol<SP>Product<SP>Operation<SP>Data".

Protocol

   This field denotes protocol version. It is fixed to "Tiny".

Product

   This field denotes product which performes action. It is used to
   segment space of available operations.
   Product must not contain spaces and it should contain only ASCII.
   Preferred format would be application name, at (@) sign followed by
   IANA assigned Private Enterprise Number. E.g. Application@12345.

Operation

   Denotes which operation is to be performed by receiver of message.
   Operation must not contain spaces and it should contain only ASCII.

Data

   JSON encoded object in form of multiple name/value pairs.
   E.g.: {"Name1":"Value1","Name2":"Value2",...,"NameN":"ValueN"}

*/
