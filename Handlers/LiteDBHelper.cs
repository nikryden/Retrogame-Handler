using LiteDB;
using Microsoft.Win32.SafeHandles;
using RetroGameHandler.Entities;
using RetroGameHandler.models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace RetroGameHandler.Handlers
{
    public class LiteDBHelper : IDisposable
    {
        public static LiteDBHelper Instance { get; set; }

        public static void init()
        {
            Instance = new LiteDBHelper();
            Instance.connect();
        }

        public static void Save<T>(T data)
        {
            if (Instance == null) init();
            Instance.Save(data);
        }

        public static ILiteQueryable<T> Load<T>()
        {
            if (Instance == null) init();
            return Instance.Load<T>();
        }

        public async static Task<ILiteQueryable<T>> LoadAsync<T>()
        {
            if (Instance == null) init();
            return await Instance.LoadAsync<T>();
        }

        public static RGHSettingsModel RGHGetData()
        {
            if (Instance == null) init();
            return Instance.RGHGetData<RGHSettingsModel>();
        }

        public static void SaveFile(string filStoragePath, string Filename, Stream InStream)
        {
            if (Instance == null) init();
            Instance.saveFile(filStoragePath, Filename, InStream);
        }

        public static void LoadFile(string filStoragePath, string Filename, Stream OutStream)
        {
            if (Instance == null) init();
            Instance.loadFile(filStoragePath, Filename, OutStream);
        }

        public static void DeleteFile(string filStoragePath, string Filename)
        {
            if (Instance == null) init();
            Instance.deleteFile(filStoragePath, Filename);
        }

        public static void DeleteFile(string onlyInFilStoragePath = null)
        {
            if (Instance == null) init();
            Instance.deleteFileAll(onlyInFilStoragePath);
        }

        public static void Delete(LiteDbEntityBase data)
        {
            if (Instance == null) init();
            Instance.delete(data);
        }

        public static void DeleteAll()
        {
            if (Instance == null) init();
            Instance.deleteAll();
        }

        public static void Close()
        {
            if (Instance == null) init();
            Instance.close();
        }

        public static void DeleteManny(BsonExpression expression)
        {
            if (Instance == null) init();
            Instance.deleteManny(expression);
        }

        public static void RGHSetData(RGHSettingsModel data)
        {
            if (Instance == null) init();
            Instance.RGHSetData<RGHSettingsModel>(data);
        }

        public void Dispose() => Dispose(true);

        private bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                db.Dispose();
                db = null;
                _safeHandle?.Dispose();
            }

            _disposed = true;
        }

        internal LiteDatabase db;

        internal void connect()
        {
            var path = @"Timeonline\RetroGameHandler";
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmpPath = Path.Combine(directory, path);
            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            db = new LiteDatabase(Path.Combine(tmpPath, @"RetroGameHandler2020.db"));
        }

        internal void close()
        {
            db.Dispose();
        }

        internal void saveFile(string filStoragePath, string Filename, Stream stream)
        {
            if (db == null) connect();
            var fs = db.FileStorage;
            fs.Upload($"$/{filStoragePath}/{Filename}", Filename, stream);
        }

        internal void deleteFile(string filStoragePath, string Filename)
        {
            if (db == null) connect();
            var fs = db.FileStorage;
            fs.Delete($"$/{filStoragePath}/{Filename}");
        }

        internal void deleteFileAll(string onlyStoragePath = null)
        {
            if (db == null) connect();
            var fs = db.FileStorage;
            foreach (var item in fs.FindAll())
            {
                if (onlyStoragePath == null || item.Id.StartsWith(onlyStoragePath))
                {
                    fs.Delete(item.Id);
                }
            }
        }

        internal void loadFile(string filStoragePath, string Filename, Stream stream)
        {
            if (db == null) connect();
            var fs = db.FileStorage;
            var file = fs.FindById($"$/{filStoragePath}/{Filename}");
            if (file != null) file.CopyTo(stream);
        }

        internal void Save<T>(T data, BsonAutoId bsonAutoId = BsonAutoId.Int32)
        {
            if (db == null) connect();
            var col = db.GetCollection<T>(bsonAutoId);
            col.Upsert(data);
        }

        internal ILiteQueryable<T> Load<T>(BsonAutoId bsonAutoId = BsonAutoId.Int32)
        {
            if (db == null) connect();
            var col = db.GetCollection<T>(bsonAutoId);
            var q = col.Query();

            return q;
        }

        internal async Task<ILiteQueryable<T>> LoadAsync<T>(BsonAutoId bsonAutoId = BsonAutoId.Int32)
        {
            return await LoadAsync<T>(CancellationToken.None, bsonAutoId);
        }

        internal async Task<ILiteQueryable<T>> LoadAsync<T>(CancellationToken cancellationToken, BsonAutoId bsonAutoId = BsonAutoId.Int32)
        {
            if (db == null) connect();
            var ts = Task.Run(() =>
             {
                 var col = db.GetCollection<T>(bsonAutoId);
                 var q = col.Query();
                 return q;
             });
            if (cancellationToken != CancellationToken.None) ts.Wait(cancellationToken);

            return await ts;
        }

        internal int ClearCollection<T>()
        {
            if (db == null) connect();
            var cl = db.GetCollection<T>(BsonAutoId.ObjectId);

            return cl.DeleteAll();
        }

        internal bool delete(LiteDbEntityBase data)
        {
            if (db == null) connect();
            var cl = db.GetCollection<LiteDbEntityBase>(BsonAutoId.ObjectId);

            return cl.Delete(data.Id);
        }

        internal int deleteAll()
        {
            if (db == null) connect();
            var cl = db.GetCollection<LiteDbEntityBase>(BsonAutoId.ObjectId);

            return cl.DeleteAll();
        }

        internal int deleteManny(BsonExpression expression)
        {
            if (db == null) connect();
            var cl = db.GetCollection<LiteDbEntityBase>(BsonAutoId.ObjectId);

            return cl.DeleteMany(expression);
        }

        internal RGHSettingsModel RGHGetData<T>()
        {
            if (db == null) connect();
            var st = db.GetCollection<RGHSettingsEntity>(BsonAutoId.ObjectId);
            var q = st.Query().ToList();
            var rm = q.FirstOrDefault();

            return new RGHSettingsModel(rm ?? new RGHSettingsEntity());
        }

        internal void RGHSetData<T>(RGHSettingsModel data)
        {
            if (db == null) connect();
            var col = db.GetCollection<RGHSettingsEntity>(BsonAutoId.ObjectId);
            var dt = new RGHSettingsEntity(data);

            //var cl = db.
            //if (data.Id == null) col.Insert(dt);
            col.Upsert(dt);
        }
    }
}