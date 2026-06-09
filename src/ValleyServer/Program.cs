#pragma warning disable SYSLIB0050

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Buffs;
using StardewValley.SaveSerialization;
using StardewValley.GameData.LocationContexts;

namespace HeadlessServer
{
    public class HeadlessContentManager : LocalizedContentManager
    {
        public HeadlessContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            string[] possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content"),

            };

            foreach (var p in possiblePaths)
            {   
                //Console.WriteLine($"[HeadlessContentManager] Checking for content directory at: {p}");
                if (!string.IsNullOrEmpty(p) && Directory.Exists(p))
                {
                    _CachedContentRoot = Path.GetFullPath(p);
                    break;
                }
            }

            if (string.IsNullOrEmpty(_CachedContentRoot))
            {
                _CachedContentRoot = Path.GetFullPath("Content");
            }

            Console.WriteLine($"[HeadlessContentManager] Content path resolved to: {_CachedContentRoot}");
        }

        public override LocalizedContentManager CreateTemporary()
        {
            return new HeadlessContentManager(base.ServiceProvider, base.RootDirectory);
        }

        protected override Stream OpenStream(string assetName)
        {
            string suffix = assetName;
            if (suffix.StartsWith("Content/", StringComparison.OrdinalIgnoreCase))
            {
                suffix = suffix.Substring(8);
            }
            else if (suffix.StartsWith("Content\\", StringComparison.OrdinalIgnoreCase))
            {
                suffix = suffix.Substring(8);
            }

            string path = Path.Combine(_CachedContentRoot, suffix);
            if (!File.Exists(path))
            {
                if (File.Exists(path + ".xnb"))
                {
                    path += ".xnb";
                }
            }

            return File.OpenRead(path);
        }

        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.Texture2D))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.Texture2D));
            }
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.SpriteFont))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.SpriteFont));
            }
            return base.Load<T>(assetName);
        }

        public override T Load<T>(string assetName, LanguageCode language)
        {
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.Texture2D))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.Texture2D));
            }
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.SpriteFont))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.SpriteFont));
            }
            return base.Load<T>(assetName, language);
        }

        public override T LoadImpl<T>(string baseAssetName, string localizedAssetName, LanguageCode languageCode)
        {
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.Texture2D))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.Texture2D));
            }
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.SpriteFont))
            {
                return (T)(object)FormatterServices.GetUninitializedObject(typeof(Microsoft.Xna.Framework.Graphics.SpriteFont));
            }
            return base.LoadImpl<T>(baseAssetName, localizedAssetName, languageCode);
        }

        public override bool DoesAssetExist<T>(string assetName)
        {
            if (typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.Texture2D) ||
                typeof(T) == typeof(Microsoft.Xna.Framework.Graphics.SpriteFont))
            {
                return true;
            }

            try
            {
                if (base.DoesAssetExist<T>(assetName))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            string suffix = assetName;
            if (suffix.StartsWith("Content/", StringComparison.OrdinalIgnoreCase))
            {
                suffix = suffix.Substring(8);
            }
            else if (suffix.StartsWith("Content\\", StringComparison.OrdinalIgnoreCase))
            {
                suffix = suffix.Substring(8);
            }

            string path = Path.Combine(_CachedContentRoot, suffix);
            if (File.Exists(path) || File.Exists(path + ".xnb"))
            {
                return true;
            }

            return false;
        }
    }

    public static class MockLidgrenMessageUtils
    {
        private static MethodInfo? writeMessageMethod = null;
        private static MethodInfo? readStreamToMessageMethod = null;

        static MockLidgrenMessageUtils()
        {
            var type = typeof(LidgrenMessageUtils);
            writeMessageMethod = type.GetMethod("WriteMessage", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            readStreamToMessageMethod = type.GetMethod("ReadStreamToMessage", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static void WriteMessage(OutgoingMessage srcMsg, NetOutgoingMessage destMsg)
        {
            writeMessageMethod?.Invoke(null, new object[] { srcMsg, destMsg });
        }

        public static void ReadStreamToMessage(NetBufferReadStream stream, IncomingMessage msg)
        {
            readStreamToMessageMethod?.Invoke(null, new object[] { stream, msg });
        }
    }

    public class HeadlessGameServer : IGameServer
    {
        private NetServer _netServer;
        private Dictionary<long, NetConnection> _clientConnections;

        public HeadlessGameServer(NetServer netServer, Dictionary<long, NetConnection> clientConnections)
        {
            _netServer = netServer;
            _clientConnections = clientConnections;
        }

        public int connectionsCount => _netServer.Connections.Count;

        public BandwidthLogger BandwidthLogger => null;
        public bool LogBandwidth { get => false; set {} }

        public string getInviteCode() => "";
        public string getUserName(long farmerId) => "Player";
        public void setPrivacy(ServerPrivacy privacy) {}
        public void stopServer() {}
        public void receiveMessages() {}
        public bool canAcceptIPConnections() => true;
        public bool canOfferInvite() => false;
        public void offerInvite() {}
        public bool connected() => true;
        public void sendMessages() {}
        public void startServer() {}
        public void initializeHost() {}
        public void sendServerIntroduction(long peer) {}
        public void kick(long disconnectee) {}
        public string ban(long farmerId) => "";
        public void playerDisconnected(long disconnectee) {}
        public bool isGameAvailable() => true;
        public bool whenGameAvailable(Action action, Func<bool> customAvailabilityCheck = null) { action(); return true; }
        public void checkFarmhandRequest(string userId, string connectionId, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage, Action approve) {}
        public void sendAvailableFarmhands(string userId, string connectionId, Action<OutgoingMessage> sendMessage) {}
        public void processIncomingMessage(IncomingMessage message) {}
        public void updateLobbyData() {}
        public float getPingToClient(long peer) => 0f;
        public bool isUserBanned(string userID) => false;
        public void onConnect(string connectionID) {}
        public void onDisconnect(string connectionID) {}
        public bool IsLocalMultiplayerInitiatedServer() => false;

        public void sendMessage(long peerId, OutgoingMessage message)
        {
            if (_clientConnections.TryGetValue(peerId, out var conn))
            {
                var msg = _netServer.CreateMessage();
                MockLidgrenMessageUtils.WriteMessage(message, msg);
                _netServer.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void sendMessage(long peerId, byte messageType, Farmer sourceFarmer, params object[] data)
        {
            this.sendMessage(peerId, new OutgoingMessage(messageType, sourceFarmer, data));
        }
    }

    class Program
    {
        static Dictionary<long, NetConnection> clientConnections = new Dictionary<long, NetConnection>();
        static string? actualProtocolVersion = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Headless Stardew Valley Server (New Farmhand Customization Stage)...");

            // Load native liblwjgl_lz4.dll
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string nativeDllPath = Path.Combine(baseDir, "liblwjgl_lz4.dll");
            string steamDirPath = @"E:\steam\steamapps\common\Stardew Valley";
            if (!File.Exists(nativeDllPath))
            {
                nativeDllPath = Path.Combine(steamDirPath, "liblwjgl_lz4.dll");
            }

            if (File.Exists(nativeDllPath))
            {
                try
                {
                    System.Runtime.InteropServices.NativeLibrary.Load(nativeDllPath);
                    Console.WriteLine($"Successfully loaded native DLL: {nativeDllPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading native DLL: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Warning: liblwjgl_lz4.dll not found locally or in default Steam path.");
            }

            // 1. Set up dependency resolution for referenced assemblies (e.g. Stardew Valley.dll)
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                string? rawName = resolveArgs.Name;
                if (rawName == null) return null;
                string? assemblyName = new AssemblyName(rawName).Name;
                if (assemblyName == null) return null;

                // Look in executable directory first
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".dll");
                if (File.Exists(path))
                {
                    try
                    {
                        return Assembly.LoadFrom(path);
                    }
                    catch {}
                }

                // Fallback to Steam path
                path = Path.Combine(steamDirPath, assemblyName + ".dll");
                if (File.Exists(path))
                {
                    try
                    {
                        return Assembly.LoadFrom(path);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error resolving/loading assembly {assemblyName}: {ex.Message}");
                    }
                }
                return null;
            };

            // 2. Extract actual protocolVersion from Game1 assembly
            try
            {
                var assembly = typeof(Game1).Assembly;
                Console.WriteLine($"Detected Game assembly at: {assembly.Location}");
                var multiplayerType = assembly.GetType("StardewValley.Multiplayer");
                if (multiplayerType != null)
                {
                    var prop = multiplayerType.GetProperty("protocolVersion", BindingFlags.Public | BindingFlags.Static);
                    if (prop != null)
                    {
                        actualProtocolVersion = prop.GetValue(null) as string;
                        Console.WriteLine($"Detected Stardew Valley Protocol Version: {actualProtocolVersion}");
                    }
                    
                    var protocolVersionOverrideField = multiplayerType.GetField("protocolVersionOverride", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (protocolVersionOverrideField != null && actualProtocolVersion != null)
                    {
                        protocolVersionOverrideField.SetValue(null, actualProtocolVersion);
                        Console.WriteLine($"Overrode Multiplayer.protocolVersionOverride with: {actualProtocolVersion}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading protocolVersion via Reflection: {ex.Message}");
            }

            string targetVersion = actualProtocolVersion ?? "1.6.15";
            Console.WriteLine($"Protocol Version to use: {targetVersion}");

            // 3. Initialize Game1 static state using uninitialized objects and reflection
            Console.WriteLine("Mocking Game1 static fields...");
            
            var serviceContainer = new GameServiceContainer();
            var headlessContent = new HeadlessContentManager(serviceContainer, "Content");
            Game1.content = headlessContent;
            
            // Create uninitialized Game1 instance to bypass constructor & XNA graphics context checks
            var gameInstance = (Game1)FormatterServices.GetUninitializedObject(typeof(Game1));
            Game1.game1 = gameInstance;
            
            var locationsField = typeof(Game1).GetField("_locations", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (locationsField != null)
            {
                var locList = new List<GameLocation>();
                locationsField.SetValue(gameInstance, locList);

                var farm = new Farm();
                farm.mapPath.Value = "Maps\\Farm";
                var loadedMapPathField = typeof(GameLocation).GetField("loadedMapPath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                loadedMapPathField?.SetValue(farm, "Maps\\Farm");
                farm.name.Value = "Farm";
                farm.isAlwaysActive.Value = true;
                locList.Add(farm);
            }
            
            var xTileContentField = typeof(Game1).GetField("xTileContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (xTileContentField != null)
            {
                xTileContentField.SetValue(gameInstance, headlessContent);
            }

            var game1Type = typeof(Game1);
            var multiplayerField = game1Type.GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic);
            Multiplayer? multiplayer = null;
            if (multiplayerField != null)
            {
                multiplayer = new Multiplayer();
                multiplayerField.SetValue(null, multiplayer);
            }

            var farmLoc = Game1.getLocationFromName("Farm");
            if (multiplayer != null && farmLoc != null)
            {
                multiplayer.locationRoot(farmLoc);
            }

            Game1.netWorldState = new NetRoot<NetWorldState>(new NetWorldState());

            // Mock Program._sdk to NullSDKHelper to prevent Steam API checks
            var programType = typeof(StardewValley.Program);
            var sdkField = programType.GetField("_sdk", BindingFlags.Static | BindingFlags.NonPublic);
            if (sdkField != null)
            {
                var nullSdkHelperType = typeof(StardewValley.SDKs.NullSDKHelper);
                var nullSdkHelper = Activator.CreateInstance(nullSdkHelperType);
                sdkField.SetValue(null, nullSdkHelper);
                Console.WriteLine("Successfully mocked Program._sdk to NullSDKHelper.");
            }

            // Initialize item registry and databases
            try
            {
                var registerMethod = typeof(ItemRegistry).GetMethod("RegisterItemTypes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                registerMethod?.Invoke(null, null);
                Console.WriteLine("ItemRegistry types registered.");

                Game1.objectData = DataLoader.Objects(Game1.content);
                Game1.bigCraftableData = DataLoader.BigCraftables(Game1.content);
                Game1.weaponData = DataLoader.Weapons(Game1.content);
                Game1.toolData = DataLoader.Tools(Game1.content);
                Game1.pantsData = DataLoader.Pants(Game1.content);
                Game1.shirtData = DataLoader.Shirts(Game1.content);
                Game1.locationContextData = DataLoader.LocationContexts(Game1.content);
                CraftingRecipe.craftingRecipes = DataLoader.CraftingRecipes(Game1.content);
                CraftingRecipe.cookingRecipes = DataLoader.CookingRecipes(Game1.content);
                Console.WriteLine("Item databases and recipes loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing ItemRegistry/databases: {ex}");
                Game1.objectData ??= new Dictionary<string, StardewValley.GameData.Objects.ObjectData>();
                Game1.bigCraftableData ??= new Dictionary<string, StardewValley.GameData.BigCraftables.BigCraftableData>();
                Game1.weaponData ??= new Dictionary<string, StardewValley.GameData.Weapons.WeaponData>();
                Game1.toolData ??= new Dictionary<string, StardewValley.GameData.Tools.ToolData>();
                Game1.pantsData ??= new Dictionary<string, StardewValley.GameData.Pants.PantsData>();
                Game1.shirtData ??= new Dictionary<string, StardewValley.GameData.Shirts.ShirtData>();
                Game1.locationContextData ??= new Dictionary<string, LocationContextData>();
                CraftingRecipe.craftingRecipes ??= new Dictionary<string, string>();
                CraftingRecipe.cookingRecipes ??= new Dictionary<string, string>();
            }

            // Create host player
            var host = new Farmer();
            host.Name = "Host";
            host.farmName.Value = "HeadlessFarm";
            host.UniqueMultiplayerID = 99999999L;
            host.isCustomized.Value = true;
            host.gameVersion = Game1.version ?? targetVersion;
            host.teamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
            
            var playerField = typeof(Game1).GetField("_player", BindingFlags.Static | BindingFlags.NonPublic);
            if (playerField != null)
            {
                playerField.SetValue(null, host);
            }
            Game1.serverHost = new NetFarmerRoot(host);

            Game1.otherFarmers = new NetRootDictionary<long, Farmer>();
            Game1.otherFarmers.Serializer = SaveSerializer.GetSerializer(typeof(Farmer));

            // Load saved farmhands from disk on startup
            LoadSavedFarmhands();

            Console.WriteLine("Game1 static fields mocked successfully!");

            // 4. Initialize Lidgren NetServer
            NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.Port = 39924;
            config.ConnectionTimeout = 30f;
            config.PingInterval = 5f;
            config.MaximumConnections = 8 * 2;
            config.MaximumTransmissionUnit = 1200;

            NetServer server = new NetServer(config);
            server.Start();
            Console.WriteLine($"Server started and listening on port {config.Port}...");
            Game1.server = new HeadlessGameServer(server, clientConnections);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long lastTickTime = 0;
            const long msPerTick = 16; // ~60 ticks per second

            // 5. Message loop
            bool running = true;
            while (running)
            {
                NetIncomingMessage inc;
                while ((inc = server.ReadMessage()) != null)
                {
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            Console.WriteLine($"Received DiscoveryRequest from {inc.SenderEndPoint}. Replying with protocol version {targetVersion}...");
                            NetOutgoingMessage response = server.CreateMessage();
                            response.Write(targetVersion);
                            response.Write("Headless Stardew Valley Server");
                            server.SendDiscoveryResponse(response, inc.SenderEndPoint);
                            break;

                        case NetIncomingMessageType.ConnectionApproval:
                            Console.WriteLine($"Received ConnectionApproval from {inc.SenderEndPoint}. Approving...");
                            inc.SenderConnection.Approve();
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)inc.ReadByte();
                            string reason = inc.ReadString();
                            Console.WriteLine($"Status changed for {inc.SenderEndPoint}: {status} (Reason: {reason})");

                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine($"Client {inc.SenderEndPoint} connected successfully. Preparing and sending available farmhands...");
                                Console.WriteLine($"Current clientConnections keys: {string.Join(", ", clientConnections.Keys)}");
                                Console.WriteLine($"Current Game1.otherFarmers.Roots keys: {string.Join(", ", Game1.otherFarmers.Roots.Keys)}");
                                
                                var availableList = new List<Farmer>();
                                
                                // Add all registered farmers in Game1.otherFarmers that are customized and not currently connected/active
                                foreach (var rootKvp in Game1.otherFarmers.Roots)
                                {
                                    var fh = rootKvp.Value.Value;
                                    if (fh != null && fh.isCustomized.Value)
                                    {
                                        Console.WriteLine($"Checking saved farmhand: Name={fh.Name}, ID={fh.UniqueMultiplayerID}, isCustomized={fh.isCustomized.Value}");
                                        if (!clientConnections.ContainsKey(fh.UniqueMultiplayerID))
                                        {
                                            availableList.Add(fh);
                                            Console.WriteLine($"Adding saved farmhand to available list: {fh.Name} ({fh.UniqueMultiplayerID})");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Skipping active farmhand: {fh.Name} ({fh.UniqueMultiplayerID})");
                                        }
                                    }
                                }

                                // If we have less than 4 farmhands, add a "New Farmhand" slot!
                                if (availableList.Count < 4)
                                {
                                    long newId = 11111111L;
                                    while (Game1.otherFarmers.Roots.ContainsKey(newId) || availableList.Any(f => f.UniqueMultiplayerID == newId))
                                    {
                                        newId += 1;
                                    }
                                    
                                    var farmhand = new Farmer();
                                    farmhand.UniqueMultiplayerID = newId;
                                    farmhand.isCustomized.Value = false;
                                    farmhand.gameVersion = Game1.version ?? targetVersion;
                                    
                                    availableList.Add(farmhand);
                                    Console.WriteLine($"Added new farmhand slot with ID: {newId}");
                                }

                                byte[] payloadBytes;
                                using (var memStream = new MemoryStream())
                                {
                                    using (var writer = new BinaryWriter(memStream))
                                    {
                                        writer.Write(1); // year
                                        writer.Write(0); // season (Spring)
                                        writer.Write(1); // day
                                        writer.Write((byte)availableList.Count); // available farmhands count
                                        
                                        foreach (var fh in availableList)
                                        {
                                            var farmhandRoot = new NetFarmerRoot(fh);
                                            farmhandRoot.WriteFull(writer);
                                        }
                                        
                                        payloadBytes = memStream.ToArray();
                                    }
                                }

                                var outgoingMsg = new OutgoingMessage(9, Game1.player.UniqueMultiplayerID, new object[] { payloadBytes });
                                var netOutgoingMsg = server.CreateMessage();
                                MockLidgrenMessageUtils.WriteMessage(outgoingMsg, netOutgoingMsg);
                                server.SendMessage(netOutgoingMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                Console.WriteLine("Sent available farmhands (Message 9) to client!");
                            }
                            else if (status == NetConnectionStatus.Disconnected || status == NetConnectionStatus.None)
                            {
                                Console.WriteLine($"Client {inc.SenderEndPoint} disconnected. Saving all active farmhands...");
                                SaveAllActiveFarmhands();

                                // Clean up connection mapping
                                long idToRemove = -1;
                                foreach (var kvp in clientConnections)
                                {
                                    if (kvp.Value == inc.SenderConnection)
                                    {
                                        idToRemove = kvp.Key;
                                        break;
                                    }
                                }
                                if (idToRemove != -1)
                                {
                                    clientConnections.Remove(idToRemove);
                                    Console.WriteLine($"Removed mapping for player {idToRemove}");

                                    // Clean up player in otherFarmers
                                    Game1.otherFarmers.TryGetValue(idToRemove, out var disconnectedFarmer);
                                    var mp = typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(null) as Multiplayer;
                                     mp?.playerDisconnected(idToRemove);

                                     // Restore the farmhand in Game1.otherFarmers from disk to make sure we have the latest saved state!
                                     string filePath = Path.Combine(savedFarmhandsPath, $"{idToRemove}.xml");
                                     if (File.Exists(filePath))
                                     {
                                         try
                                         {
                                             using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                             {
                                                 var serializer = SaveSerializer.GetSerializer(typeof(Farmer));
                                                 var farmer = serializer.Deserialize(stream) as Farmer;
                                                 if (farmer != null)
                                                 {
                                                     var root = new NetFarmerRoot(farmer);
                                                     Game1.otherFarmers.Roots[idToRemove] = root;
                                                     Console.WriteLine($"Successfully reloaded and re-registered farmhand {idToRemove} from disk.");
                                                 }
                                             }
                                         }
                                         catch (Exception ex)
                                         {
                                             Console.WriteLine($"Error reloading farmhand {idToRemove} on disconnect: {ex}");
                                             if (disconnectedFarmer != null)
                                             {
                                                 Game1.otherFarmers.Roots[idToRemove] = new NetFarmerRoot(disconnectedFarmer);
                                             }
                                         }
                                     }
                                     else if (disconnectedFarmer != null)
                                     {
                                         Game1.otherFarmers.Roots[idToRemove] = new NetFarmerRoot(disconnectedFarmer);
                                         Console.WriteLine($"Re-registered disconnected farmer {idToRemove} from memory.");
                                     }

                                    // Notify other clients about the disconnection
                                    if (disconnectedFarmer != null)
                                    {
                                        var discMsg = new OutgoingMessage(19, disconnectedFarmer);
                                        foreach (var connKvp in clientConnections)
                                        {
                                            if (connKvp.Key != idToRemove)
                                            {
                                                var msg = server.CreateMessage();
                                                MockLidgrenMessageUtils.WriteMessage(discMsg, msg);
                                                server.SendMessage(msg, connKvp.Value, NetDeliveryMethod.ReliableOrdered);
                                            }
                                        }
                                        Console.WriteLine($"Broadcasted player {idToRemove} disconnect (Message 19) to remaining clients.");
                                    }
                                }
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            IncomingMessage incomingMsg = new IncomingMessage();
                            using (NetBufferReadStream stream = new NetBufferReadStream(inc))
                            {
                                while (inc.LengthBits - inc.Position >= 8)
                                {
                                    MockLidgrenMessageUtils.ReadStreamToMessage(stream, incomingMsg);
                                     if (incomingMsg.MessageType == 2)
                                     {
                                         Console.WriteLine("Received PlayerIntroduction (Message 2) from client!");
                                         
                                         var clientFarmerRoot = new NetFarmerRoot();
                                         clientFarmerRoot.ReadConnectionPacket(incomingMsg.Reader);
                                         var clientFarmer = clientFarmerRoot.Value;
                                         long newClientId = clientFarmer.UniqueMultiplayerID;
                                         Console.WriteLine($"Client requested farmhand ID: {newClientId}, Name: {clientFarmer.Name}");

                                         // Register client farmhand
                                         Game1.otherFarmers.Roots[newClientId] = clientFarmerRoot;

                                         // Map connection
                                         clientConnections[newClientId] = inc.SenderConnection;
                                         Console.WriteLine($"Mapped connection for player {newClientId}");

                                         // Send ServerIntroduction (Message 1)
                                         Console.WriteLine("Sending ServerIntroduction (Message 1)...");
                                         byte[] hostBytes = WriteObjectFullBytes(Game1.serverHost, newClientId);
                                         byte[] teamBytes = WriteObjectFullBytes(Game1.player.teamRoot, newClientId);
                                         byte[] worldStateBytes = WriteObjectFullBytes(Game1.netWorldState, newClientId);

                                         var introMsg = new OutgoingMessage(1, Game1.player.UniqueMultiplayerID, new object[] { hostBytes, teamBytes, worldStateBytes });
                                         var netIntroMsg = server.CreateMessage();
                                         MockLidgrenMessageUtils.WriteMessage(introMsg, netIntroMsg);
                                         server.SendMessage(netIntroMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                         Console.WriteLine("Sent ServerIntroduction!");

                                         // Send LocationIntroduction (Message 3) for "Farm" location with force_current = true
                                         Console.WriteLine("Sending LocationIntroduction (Message 3)...");
                                         var location = Game1.getLocationFromName("Farm");
                                         var locRoot = new NetRoot<GameLocation>();
                                         locRoot.Set(location);
                                         byte[] locationBytes = WriteObjectFullBytes(locRoot, newClientId);

                                         var locMsg = new OutgoingMessage(3, Game1.player.UniqueMultiplayerID, new object[] { true, locationBytes });
                                         var netLocMsg = server.CreateMessage();
                                         MockLidgrenMessageUtils.WriteMessage(locMsg, netLocMsg);
                                         server.SendMessage(netLocMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                         Console.WriteLine("Sent LocationIntroduction!");

                                         // Introduce new client to existing clients, and vice versa
                                         foreach (var rootKvp in Game1.otherFarmers.Roots)
                                         {
                                             long otherId = rootKvp.Key;
                                             if (otherId != newClientId && otherId != 99999999L && otherId != 0)
                                             {
                                                 if (clientConnections.TryGetValue(otherId, out var otherConn))
                                                 {
                                                     // 1. Send new client's introduction to existing client (otherId)
                                                     Console.WriteLine($"Introducing new player {newClientId} to existing player {otherId}...");
                                                     byte[] newClientBytes = WriteObjectFullBytes(clientFarmerRoot, otherId);
                                                     var introToExisting = new OutgoingMessage(2, clientFarmer, new object[] { "Player", newClientBytes });
                                                     var netMsgToExisting = server.CreateMessage();
                                                     MockLidgrenMessageUtils.WriteMessage(introToExisting, netMsgToExisting);
                                                     server.SendMessage(netMsgToExisting, otherConn, NetDeliveryMethod.ReliableOrdered);

                                                     // 2. Send existing client's introduction to new client (newClientId)
                                                     Console.WriteLine($"Introducing existing player {otherId} to new player {newClientId}...");
                                                     byte[] existingClientBytes = WriteObjectFullBytes(rootKvp.Value, newClientId);
                                                     var introToNew = new OutgoingMessage(2, rootKvp.Value.Value, new object[] { "Player", existingClientBytes });
                                                     var netMsgToNew = server.CreateMessage();
                                                     MockLidgrenMessageUtils.WriteMessage(introToNew, netMsgToNew);
                                                     server.SendMessage(netMsgToNew, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                                 }
                                             }
                                         }
                                     }
                                    else if (incomingMsg.MessageType == 5)
                                    {
                                        try
                                        {
                                            short x = incomingMsg.Reader.ReadInt16();
                                            short y = incomingMsg.Reader.ReadInt16();
                                            string name = incomingMsg.Reader.ReadString();
                                            byte flags = incomingMsg.Reader.ReadByte();
                                            bool isStructure = (flags & 1) != 0;
                                            bool needsLocationInfo = (flags & 4) != 0;

                                            var farmer = incomingMsg.SourceFarmer;
                                            if (farmer != null && needsLocationInfo)
                                            {
                                                 GameLocation? location = Game1.getLocationFromName(name, isStructure);
                                                 if (location == null)
                                                 {
                                                     Console.WriteLine($"Location '{name}' not found on server. Instantiating on the fly...");
                                                     if (name == "FarmHouse")
                                                     {
                                                         location = new StardewValley.Locations.FarmHouse();
                                                     }
                                                     else
                                                     {
                                                         location = new GameLocation();
                                                     }
                                                     location.mapPath.Value = "Maps\\" + name;
                                                     var loadedMapPathField = typeof(GameLocation).GetField("loadedMapPath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                                     loadedMapPathField?.SetValue(location, "Maps\\" + name);
                                                     location.name.Value = name;
                                                     location.isAlwaysActive.Value = true;
                                                     
                                                     var locList = typeof(Game1).GetField("_locations", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(Game1.game1) as List<GameLocation>;
                                                     locList?.Add(location);
                                                 }
                                                 farmer.currentLocation = location;
                                                 farmer.Position = new Vector2(x * 64, y * 64 - (farmer.Sprite.getHeight() - 32) + 16);

                                                var locRoot = new NetRoot<GameLocation>();
                                                locRoot.Set(location);
                                                byte[] locationBytes = WriteObjectFullBytes(locRoot, farmer.UniqueMultiplayerID);

                                                var locMsg = new OutgoingMessage(3, Game1.player.UniqueMultiplayerID, new object[] { false, locationBytes });
                                                var netLocMsg = server.CreateMessage();
                                                MockLidgrenMessageUtils.WriteMessage(locMsg, netLocMsg);
                                                server.SendMessage(netLocMsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                                Console.WriteLine($"Sent warp location info for {name} to client!");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error processing warp: {ex.Message}");
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                             var mp = typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(null) as Multiplayer;
                                             mp?.processIncomingMessage(incomingMsg);
                                            
                                            // Check if the source farmer has completed customization and needs saving
                                            var farmer = incomingMsg.SourceFarmer;
                                            if (farmer != null && farmer.UniqueMultiplayerID != 99999999L && farmer.UniqueMultiplayerID != 0 && farmer.isCustomized.Value && !savedFarmerIds.Contains(farmer.UniqueMultiplayerID))
                                            {
                                                Console.WriteLine($"Farmer {farmer.Name} ({farmer.UniqueMultiplayerID}) completed customization. Saving...");
                                                SaveFarmhand(farmer);
                                                savedFarmerIds.Add(farmer.UniqueMultiplayerID);
                                            }

                                            // Rebroadcast client broadcast messages to other clients
                                             if (mp?.isClientBroadcastType(incomingMsg.MessageType) ?? false)
                                            {
                                                var outMsg = new OutgoingMessage(incomingMsg);
                                                BroadcastMessage(outMsg, server, inc.SenderConnection);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error processing message {incomingMsg.MessageType}: {ex.Message}");
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    server.Recycle(inc);
                }

                long currentTime = stopwatch.ElapsedMilliseconds;
                if (currentTime - lastTickTime >= msPerTick)
                {
                    lastTickTime = currentTime;
                    Game1.ticks++;

                    if (Game1.netWorldState != null)
                    {
                        Game1.netWorldState.Value.UpdateFromGame1();
                    }

                    try
                    {
                         var mp = typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(null) as Multiplayer;
                         if (mp != null)
                         {
                             mp.UpdateEarly();
                             mp.UpdateLate();
                         }
                    }
                    catch (Exception)
                    {
                    }

                    // Forward queued messages
                    foreach (var farmer in Game1.otherFarmers.Values)
                    {
                        if (farmer.messageQueue.Count > 0 && clientConnections.TryGetValue(farmer.UniqueMultiplayerID, out var conn))
                        {
                            foreach (var outMsg in farmer.messageQueue)
                            {
                                var msg = server.CreateMessage();
                                MockLidgrenMessageUtils.WriteMessage(outMsg, msg);
                                server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
                            }
                            farmer.messageQueue.Clear();
                        }
                    }

                    if (Game1.player != null && Game1.player.messageQueue.Count > 0)
                    {
                        foreach (var outMsg in Game1.player.messageQueue)
                        {
                            foreach (var connKvp in clientConnections)
                            {
                                var msg = server.CreateMessage();
                                MockLidgrenMessageUtils.WriteMessage(outMsg, msg);
                                server.SendMessage(msg, connKvp.Value, NetDeliveryMethod.ReliableOrdered);
                            }
                        }
                        Game1.player.messageQueue.Clear();
                    }
                }
                Thread.Sleep(1);
            }
        }

        private static string savedFarmhandsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saved_farmhands");
        private static HashSet<long> savedFarmerIds = new HashSet<long>();

        static List<Farmer> LoadSavedFarmhands()
        {
            var list = new List<Farmer>();
            if (!Directory.Exists(savedFarmhandsPath))
            {
                Directory.CreateDirectory(savedFarmhandsPath);
            }

            foreach (var file in Directory.GetFiles(savedFarmhandsPath, "*.xml"))
            {
                try
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var serializer = SaveSerializer.GetSerializer(typeof(Farmer));
                        var farmer = serializer.Deserialize(stream) as Farmer;
                        if (farmer != null && farmer.UniqueMultiplayerID != 99999999L && farmer.UniqueMultiplayerID != 0)
                        {
                            list.Add(farmer);
                            savedFarmerIds.Add(farmer.UniqueMultiplayerID);
                            
                            // Also register it in Game1.otherFarmers
                            var root = new NetFarmerRoot(farmer);
                            Game1.otherFarmers.Roots[farmer.UniqueMultiplayerID] = root;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading saved farmhand from {file}: {ex}");
                }
            }
            return list;
        }

        static void SaveFarmhand(Farmer farmer)
        {
            if (!Directory.Exists(savedFarmhandsPath))
            {
                Directory.CreateDirectory(savedFarmhandsPath);
            }

            string filePath = Path.Combine(savedFarmhandsPath, $"{farmer.UniqueMultiplayerID}.xml");
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    var serializer = SaveSerializer.GetSerializer(typeof(Farmer));
                    serializer.Serialize(stream, farmer);
                }
                Console.WriteLine($"Successfully saved farmhand {farmer.Name} ({farmer.UniqueMultiplayerID}) to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving farmhand {farmer.Name}: {ex.Message}");
            }
        }

        static void SaveAllActiveFarmhands()
        {
            foreach (var rootKvp in Game1.otherFarmers.Roots)
            {
                var farmer = rootKvp.Value.Value;
                if (farmer != null && farmer.UniqueMultiplayerID != 99999999L && farmer.UniqueMultiplayerID != 0 && farmer.isCustomized.Value)
                {
                    SaveFarmhand(farmer);
                }
            }
        }

        static void BroadcastMessage(OutgoingMessage outMsg, NetServer server, NetConnection? excludeConnection = null)
        {
            if (server.Connections.Count == 0) return;
            
            var msg = server.CreateMessage();
            MockLidgrenMessageUtils.WriteMessage(outMsg, msg);
            
            List<NetConnection> targets = new List<NetConnection>();
            foreach (var conn in server.Connections)
            {
                if (conn != excludeConnection && conn.Status == NetConnectionStatus.Connected)
                {
                    targets.Add(conn);
                }
            }

            if (targets.Count > 0)
            {
                server.SendMessage(msg, targets, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        static byte[] WriteObjectFullBytes<T>(NetRoot<T> root, long peer) where T : class, INetObject<INetSerializable>
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    root.CreateConnectionPacket(writer, peer);
                    return stream.ToArray();
                }
            }
        }
    }
}
