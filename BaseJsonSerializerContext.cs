using M56X.Core.Enums;
using M56X.Core.Executor;
using M56X.Core.Model;
using System.Collections;
using System.Drawing;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace M56X.Core.Json
{
    /// <summary>
    /// 
    /// </summary>
    [JsonSerializable(typeof(object))]
    [JsonSerializable(typeof(object[]))]
    [JsonSerializable(typeof(DynamicObject))]

    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(TimeSpan))]
    [JsonSerializable(typeof(DateTimeOffset))]

    [JsonSerializable(typeof(NetStatistics))]

    [JsonSerializable(typeof(Task))]
    [JsonSerializable(typeof(Task<>))]

    [JsonSerializable(typeof(Enum))]
    [JsonSerializable(typeof(ValueTuple))]
    [JsonSerializable(typeof((string, string)))]
    [JsonSerializable(typeof((object, object)))]
    [JsonSerializable(typeof((bool, bool)))]
    [JsonSerializable(typeof((uint, uint)))]
    [JsonSerializable(typeof((int, int)))]
    [JsonSerializable(typeof((ulong, ulong)))]
    [JsonSerializable(typeof((long, long)))]
    [JsonSerializable(typeof((float, float)))]
    [JsonSerializable(typeof((double, double)))]

    [JsonSerializable(typeof((string, string, string)))]
    [JsonSerializable(typeof((object, object, object)))]
    [JsonSerializable(typeof((bool, bool, bool)))]
    [JsonSerializable(typeof((uint, uint, uint)))]
    [JsonSerializable(typeof((int, int, int)))]
    [JsonSerializable(typeof((ulong, ulong, ulong)))]
    [JsonSerializable(typeof((long, long, long)))]
    [JsonSerializable(typeof((float, float, float)))]
    [JsonSerializable(typeof((double, double, double)))]

    [JsonSerializable(typeof(JsonElement))]
    [JsonSerializable(typeof(JsonNode))]
    [JsonSerializable(typeof(JsonDocument))]
    

    [JsonSerializable(typeof(List<object>))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, Dictionary<string, object?>>))]

    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(char))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(byte))]
    [JsonSerializable(typeof(sbyte))]
    [JsonSerializable(typeof(ushort))]
    [JsonSerializable(typeof(short))]
    [JsonSerializable(typeof(uint))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(ulong))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(float))]
    [JsonSerializable(typeof(double))]

    [JsonSerializable(typeof(Array))]
    [JsonSerializable(typeof(DictionaryBase))]
    [JsonSerializable(typeof(bool[]))]
    [JsonSerializable(typeof(char[]))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(byte[]))]
    [JsonSerializable(typeof(sbyte[]))]
    [JsonSerializable(typeof(ushort[]))]
    [JsonSerializable(typeof(short[]))]
    [JsonSerializable(typeof(uint[]))]
    [JsonSerializable(typeof(int[]))]
    [JsonSerializable(typeof(ulong[]))]
    [JsonSerializable(typeof(long[]))]
    [JsonSerializable(typeof(float[]))]
    [JsonSerializable(typeof(double[]))]

    [JsonSerializable(typeof(Guid))]
    [JsonSerializable(typeof(Decimal))]
    [JsonSerializable(typeof(Color))]
    [JsonSerializable(typeof(KnownColor))]
    [JsonSerializable(typeof(ConsoleColor))]
    [JsonSerializable(typeof(Point))]
    [JsonSerializable(typeof(PointF))]
    [JsonSerializable(typeof(Size))]
    [JsonSerializable(typeof(SizeF))]
    [JsonSerializable(typeof(Rectangle))]
    [JsonSerializable(typeof(RectangleF))]

    [JsonSerializable(typeof(EndiannessType))]
    [JsonSerializable(typeof(DataFlowDirection))]

    [JsonSerializable(typeof(ApiResult))]

    [JsonSerializable(typeof(ActionExecuteMode))]
    [JsonSerializable(typeof(ActionTriggerType))]
    [JsonSerializable(typeof(ActionExecutorGroup))]
    [JsonSerializable(typeof(ActionItemBase))]
    [JsonSerializable(typeof(ActionItemBase[]))]
    [JsonSerializable(typeof(List<ActionItemBase>))]
    [JsonSerializable(typeof(ActionExecutor<ActionItemBase>))]
    [JsonSerializable(typeof(List<ActionExecutor<ActionItemBase>>))]
    [JsonSerializable(typeof(ActionExecutor<ActionItemBase>[]))]

    public partial class BaseJsonSerializerContext : JsonSerializerContext { }
}
