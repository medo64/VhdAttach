//Copyright (c) 2011 Josip Medved <jmedved@jmedved.com>

//2011-08-26: Initial version (based on TinyMessage).
//2011-10-22: Adjusted to work on Mono.
//            Added IsListening property.
//2011-10-24: Added UseObjectEncoding.
//2011-11-07: Fixing encoding/decoding.
//            Changed all parsing errors to throw FormatException.


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
    public class TinyPair : IDisposable {

        /// <summary>
        /// Default port for TinyMessage protocol.
        /// </summary>
        public static int DefaultPort { get { return 5104; } }


        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyPair()
            : this(new IPEndPoint(IPAddress.Any, TinyPair.DefaultPort)) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="localEndPoint">Local end point where messages should be received at.</param>
        /// <exception cref="System.ArgumentNullException">Local IP end point is null.</exception>
        public TinyPair(IPEndPoint localEndPoint) {
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
                this.ListenThread = new Thread(Run) { IsBackground = true, Name = "TinyPair " + this.LocalEndPoint.ToString() };
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
        /// Gets whether TinyPair is in listening state.
        /// </summary>
        public bool IsListening {
            get { return (this.ListenThread != null) && (this.ListenThread.IsAlive); }
        }

        /// <summary>
        /// Raises event when packet arrives.
        /// </summary>
        public event EventHandler<TinyPairPacketEventArgs> TinyPairPacketReceived;


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

                    if (TinyPairPacketReceived != null) {
                        var newBuffer = new byte[inCount];
                        Buffer.BlockCopy(buffer, 0, newBuffer, 0, inCount);
                        Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyPair [{0} <- {1}]", TinyPairPacket.ParseHeaderOnly(newBuffer, 0, inCount), remoteEP));
                        var invokeArgs = new object[] { this, new TinyPairPacketEventArgs(newBuffer, 0, inCount, remoteEP as IPEndPoint) };
                        foreach (Delegate iDelegate in TinyPairPacketReceived.GetInvocationList()) {
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
        public static void Send(TinyPairPacket packet, IPAddress address) {
            Send(packet, new IPEndPoint(address, TinyPair.DefaultPort));
        }

        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <param name="port">Port of destination for packet.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static void Send(TinyPairPacket packet, IPAddress address, int port) {
            Send(packet, new IPEndPoint(address, port));
        }

        /// <summary>
        /// Sends UDP packet.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="remoteEndPoint">Address of destination for packet. It can be broadcast address.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static void Send(TinyPairPacket packet, IPEndPoint remoteEndPoint) {
            if (packet == null) { throw new ArgumentNullException("packet", "Packet is null."); }
            if (remoteEndPoint == null) { throw new ArgumentNullException("remoteEndPoint", "Remote IP end point is null."); }
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                if (remoteEndPoint.Address == IPAddress.Broadcast) {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                }
                if (IsRunningOnMono == false) { socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoChecksum, false); }
                socket.SendTo(packet.GetBytes(), remoteEndPoint);
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyPair [{0} -> {1}]", packet, remoteEndPoint));
            }
        }


        /// <summary>
        /// Sends one UDP packet and expects another in return.
        /// Returns null if respose is not received within 250 milliseconds.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <param name="address">IP address of destination for packet. It can be broadcast address.</param>
        /// <exception cref="System.ArgumentNullException">Packet is null. -or- Remote IP end point is null.</exception>
        public static TinyPairPacket SendAndReceive(TinyPairPacket packet, IPAddress address) {
            return SendAndReceive(packet, new IPEndPoint(address, TinyPair.DefaultPort), 250);
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
        public static TinyPairPacket SendAndReceive(TinyPairPacket packet, IPAddress address, int port) {
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
        public static TinyPairPacket SendAndReceive(TinyPairPacket packet, IPEndPoint remoteEndPoint, int receiveTimeout) {
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
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyPair [{0} -> {1}]", packet, remoteEndPoint));

                EndPoint remoteEndPointIn = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                var bytesIn = new byte[65536];
                try {
                    int len = socket.ReceiveFrom(bytesIn, ref remoteEndPointIn);
                    var packetIn = Medo.Net.TinyPairPacket.Parse(bytesIn, 0, len);
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TinyPair [{0} <- {1}]", packetIn, remoteEndPointIn));
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
    /// Encoder/decoder for TinyPair packets.
    /// </summary>
    public class TinyPairPacket {

        private static readonly UTF8Encoding TextEncoding = new UTF8Encoding(false);

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="product">Name of product. Preferred format would be application name, at (@) sign, IANA assigned Private Enterprise Number. E.g. Application@12345</param>
        /// <param name="operation">Message type.</param>
        /// <param name="data">Data to be encoded in JSON.</param>
        /// <exception cref="System.ArgumentNullException">Product is null or empty. -or- Operation is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Product contains space character. -or- Operation contains space character.</exception>
        public TinyPairPacket(string product, string operation, Dictionary<string, string> data) {
            if (string.IsNullOrEmpty(product)) { throw new ArgumentNullException("product", "Product is null or empty."); }
            if (product.Contains(" ")) { throw new ArgumentException("Product contains space character.", "product"); }
            if (string.IsNullOrEmpty(operation)) { throw new ArgumentNullException("operation", "Operation is null or empty."); }
            if (operation.Contains(" ")) { throw new ArgumentException("Operation contains space character.", "operation"); }

            this.Product = product;
            this.Operation = operation;
            this.Data = data;
        }

        /// <summary>
        /// Gets name of product.
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Gets operation.
        /// </summary>
        public string Operation { get; private set; }

        /// <summary>
        /// Gets data object.
        /// </summary>
        public Dictionary<string, string> Data { get; private set; }


        /// <summary>
        /// Gets/sets whether data will be encoded as dictionary-style JSON (false) or in object notation (true).
        /// False (default) should only be used for compatibility with older versions of this encoder.
        /// </summary>
        public bool UseObjectEncoding { get; set; }


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

                if (this.UseObjectEncoding) {
                    var addComma = false;
                    if (this.Data != null) {
                        stream.Write(new byte[] { 0x7B }, 0, 1); //{
                        foreach (var item in this.Data) {
                            byte[] keyBytes = TextEncoding.GetBytes(JsonEncode(item.Key));
                            byte[] valueBytes = TextEncoding.GetBytes(JsonEncode(item.Value));
                            if (addComma) { stream.Write(new byte[] { 0x2C }, 0, 1); } //,
                            stream.Write(new byte[] { 0x22 }, 0, 1); //"
                            stream.Write(keyBytes, 0, keyBytes.Length);
                            stream.Write(new byte[] { 0x22, 0x3A, 0x22 }, 0, 3); //":"
                            stream.Write(valueBytes, 0, valueBytes.Length);
                            stream.Write(new byte[] { 0x22 }, 0, 1); //"
                            addComma = true;
                        }
                        stream.Write(new byte[] { 0x7D }, 0, 1); //}
                    } else {
                        stream.Write(new byte[] { 0x6E, 0x75, 0x6C, 0x6C }, 0, 4); //null
                    }
                } else {
                    var addComma = false;
                    stream.Write(new byte[] { 0x5B }, 0, 1); //[
                    if (this.Data != null) {
                        foreach (var item in this.Data) {
                            byte[] keyBytes = TextEncoding.GetBytes(JsonEncode(item.Key));
                            byte[] valueBytes = TextEncoding.GetBytes(JsonEncode(item.Value));
                            if (addComma) { stream.Write(new byte[] { 0x2C }, 0, 1); } //,
                            stream.Write(new byte[] { 0x7B, 0x22, 0x4B, 0x65, 0x79, 0x22, 0x3A, 0x22 }, 0, 8); //"{Key":"
                            stream.Write(keyBytes, 0, keyBytes.Length);
                            stream.Write(new byte[] { 0x22, 0x2C, 0x22, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x22, 0x3A, 0x22 }, 0, 11); //","Value":"
                            stream.Write(valueBytes, 0, valueBytes.Length);
                            stream.Write(new byte[] { 0x22, 0x7D }, 0, 2); //"}
                            addComma = true;
                        }
                    }
                    stream.Write(new byte[] { 0x5D }, 0, 1); //]
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
        public static TinyPairPacket Parse(byte[] buffer) {
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
        public static TinyPairPacket ParseHeaderOnly(byte[] buffer, int offset, int count) {
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

                return new TinyPairPacket(product, operation, null);
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
        public static TinyPairPacket Parse(byte[] buffer, int offset, int count) {
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
                        } else if (ch == '[') {
                            ParseJsonArray(jsonText, data);
                            break;
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
                return new TinyPairPacket(product, operation, data);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cyclomatic complexity is actually lower than code analysis shows.")]
        private static JsonState ParseJsonArray(Queue<char> jsonText, Dictionary<string, string> data) {
            var nameValuePairs = new Dictionary<string, string>();
            var state = JsonState.Default;
            var sbName = new StringBuilder();
            var sbValue = new StringBuilder();
            while (jsonText.Count > 0) {
                var ch = jsonText.Dequeue();
                switch (state) {
                    case JsonState.Default: {
                            switch (ch) {
                                case ' ': break;
                                case '[': state = JsonState.LookingForObjectStart; break;
                                default: throw new System.FormatException("Cannot find array start.");
                            }
                        } break;

                    case JsonState.LookingForObjectStart: {
                            switch (ch) {
                                case ' ': break;
                                case '{': state = JsonState.LookingForNameStart; break;
                                case ']': state = JsonState.DeadEnd; break;
                                default: throw new System.FormatException("Cannot find item start.");
                            }
                        } break;

                    case JsonState.LookingForNameStart: {
                            switch (ch) {
                                case ' ': break;
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
                                default: throw new System.FormatException("Cannot find key value start.");
                            }
                        } break;

                    case JsonState.LookingForValueEnd: {
                            switch (ch) {
                                case '\\': sbValue.Append(Descape(jsonText)); break;
                                case '\"':
                                    nameValuePairs.Add(sbName.ToString(), sbValue.ToString());
                                    sbName.Length = 0;
                                    sbValue.Length = 0;
                                    state = JsonState.LookingForObjectEnd;
                                    break;
                                default: sbValue.Append(ch); break;
                            }
                        } break;

                    case JsonState.LookingForObjectEnd: {
                            switch (ch) {
                                case ' ': break;
                                case ',': state = JsonState.LookingForNameStart; break;
                                case '}':
                                    if (nameValuePairs.ContainsKey("Key") && nameValuePairs.ContainsKey("Value")) {
                                        data.Add(nameValuePairs["Key"], nameValuePairs["Value"]);
                                    } else {
                                        throw new System.FormatException("Cannot find key and value.");
                                    }
                                    nameValuePairs.Clear();
                                    state = JsonState.LookingForObjectSeparator;
                                    break;
                                default: throw new System.FormatException("Cannot find item start.");
                            }
                        } break;

                    case JsonState.LookingForObjectSeparator: {
                            switch (ch) {
                                case ' ': break;
                                case ',': state = JsonState.LookingForObjectStart; break;
                                case ']': state = JsonState.Default; break;
                                default: throw new System.FormatException("Cannot find item separator start.");
                            }
                        } break;

                    case JsonState.DeadEnd: {
                            switch (ch) {
                                case ' ': break;
                                default: throw new System.FormatException("Unexpected data.");
                            }
                        } break;

                }
            }
            return state;
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
                                default: throw new System.FormatException("Cannot find key value start.");
                            }
                        } break;

                    case JsonState.LookingForValueEnd: {
                            switch (ch) {
                                case '\\': sbValue.Append(Descape(jsonText)); break;
                                case '\"':
                                    data.Add(sbName.ToString(), sbValue.ToString());
                                    sbName.Length = 0;
                                    sbValue.Length = 0;
                                    state = JsonState.LookingForObjectEnd;
                                    break;
                                default: sbValue.Append(ch); break;
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
            DeadEnd,
        }

    }



    /// <summary>
    /// Event arguments for TinyPairPacketReceived message.
    /// </summary>
    public class TinyPairPacketEventArgs : EventArgs {

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
        public TinyPairPacketEventArgs(byte[] buffer, int offset, int count, IPEndPoint remoteEndPoint) {
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
        public TinyPairPacket GetPacket() {
            return TinyPairPacket.Parse(this.Buffer, this.Offset, this.Count);
        }

        /// <summary>
        /// Returns parsed packet.
        /// </summary>
        /// <exception cref="System.FormatException">Cannot parse packet.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is appropriate here.")]
        public TinyPairPacket GetPacketWithoutData() {
            return TinyPairPacket.ParseHeaderOnly(this.Buffer, this.Offset, this.Count);
        }

    }
}
