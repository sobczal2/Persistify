using System;
using Newtonsoft.Json;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Persistify.DataStructures.MultiTargetTries.JsonConverters;

public class MultiTargetTrieJsonConverter<TItem> : JsonConverter<IMultiTargetTrie<TItem>>
    where TItem : notnull
{
    public override void WriteJson(JsonWriter writer, IMultiTargetTrie<TItem>? value, JsonSerializer serializer)
    {
        if (value is MultiTargetTrie<TItem> multiTargetTrie)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("AlphabetSize");
            var alphabetSize = (int)(byte)multiTargetTrie.GetType().GetField("_alphabetSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(multiTargetTrie);
            writer.WriteValue(alphabetSize);
            writer.WritePropertyName("Root");
            var root = multiTargetTrie.GetType().GetField("_root", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(multiTargetTrie)!;
            serializer.Serialize(writer, root);
            writer.WriteEndObject();
        }
    }

    public override IMultiTargetTrie<TItem>? ReadJson(JsonReader reader, Type objectType, IMultiTargetTrie<TItem>? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        reader.Read();
        var alphabetSize = reader.ReadAsInt32();
        reader.Read();
        var root = serializer.Deserialize<MultiTargetTrieNode<TItem>>(reader);
        reader.Read();
        
        return new MultiTargetTrie<TItem>((byte)alphabetSize, root);
    }
}