ULTIMATE JSON by Tahion Studios

1)DESCRIPTION:

Best JSON Serializer/Deserializer on UNITY. Two times faster as the Newtonsoft(NETJson). 
Based on FastJson.

Android, iOS, Windows Phone, Windows, Mac OS, Linux

2)FEATURES:

- faster JSON Serializer/Deserializer on UNITY;
- based on FastJSON;
- JSON Framework for comfort working with JSON;
- 2 method for compress/decompress JSON: LZMA and QZIP
- Support UTF8 fields of JSON
- Full source code
- Multiplatform plugin: Android, iOS, Windows Phone, Windows, Mac OS, Linux

SupportedTypes (Deserilise with JsonObject):
- Long
- Double
- Bool
- String
- List
- Dictionary

SupportedTypes (Deserilise with generic T):
Unity build-in types:
- Vector2
- Vector2Int
- Vector3
- Vector3Int
- Vector4
- Color
- Color32
- Rect
- RectInt
- Bounds
- BoundsInt
- Quaternion
- Ray
- Ray2D

C# types:
- Int
- Long
- String
- Bool
- DateTime
- Enum
- Guid
- StringDictionary
- NameValueCollection
- Array
- ByteArray
- Dictionary
- Hashtable

3)TUTORIAL:

a) Go to Edit->UltimateJSON->Copy Dll To Project
b) Go to BuildSettings->PlayerSettings->OtherSettings->Api Compatibility Level = "Net 2.0" (without subset)
c) If you use Mac or part a doesn't work(if we use 2 or more unity version on one computer (ex. 5.5.0f3 and 5.4.0) or you use Unity x86 version not x64):
   c1)Copy System.Data.dll to your Asset Root Folder:
   c2)If you use Windows go to UNITY_FOLDER/Editor/Data/Mono/lib/mono/2.0/System.Data.dll ex.: UNITY_FOLDER: "C://ProgramFiles/Unity"
   c3)For MacOS: Application/Unity.Unity.app ->Righ Button Mouse Click->Show Package Contents-> Contents/Frameworks/Mono/lib/mono/2.0/System.Data.dll
d) Tutorial Scene - YOUR_UNITY_PROJECT_FOLDER/UltimateJSON/ExampleScene/1.unity

3)METHODS:

JSONObject:

//serialise System.object to JsonString
//addAssemblyTypes - add filed with assembly type in JSON
- static string Serialise(object obj, bool addAssemblyTypes = false);

//deserialise jsonString to System.object T
//addAssemblyTypes - add filed with assembly type in JSON
- static T Deserialise<T>(string jsonString, bool addAssemblyTypes = false)

//deserialise jsonString to JsonObject
- static JsonObject Deserialise(string jsonString);

//get: JsonObject["personages"]["1"]["preview"].ToString ()
//set: JsonObject["personages"]["1"]["preview"] = new JsonObject("ff");
- JsonObject [string i] {get; set;};

//for get or set list
- JsonObject [int i] {get; set;};

//Get object value from JSON, ex.:JsonObject["preview"].TryGetValue<int>(); -return (int)1;
- T TryGetValue<T>();

//IEnumerator for foreach, ex.: foreach (var field in JsonObject["personages"]){ }
- IEnumerator GetEnumerator();

//Add custom type to UltimateJSON
- void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer);

JSONCompress:

//Compress Json if(best = true) Json compress with LZMA Algorithm else compress with gzip
- static byte[] CompressJson(string json, bool best = true)


//DeCompress Json if(best = true) Json decompress with LZMA Algorithm else decompress with gzip
- static string DecompressJson(byte[] sourceData, bool best = true)

4)CONTACTS:

http://tachyon.byethost12.com/

All of your comments and suggestions please send on email:
tahionstudios@gmail.com
All right reserved by Tahion Studios