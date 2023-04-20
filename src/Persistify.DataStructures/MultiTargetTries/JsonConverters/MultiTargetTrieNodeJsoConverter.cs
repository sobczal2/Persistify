using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Persistify.DataStructures.MultiTargetTries.MultitargetTrieByteTranslationTrie;

namespace Persistify.DataStructures.MultiTargetTries.JsonConverters;

public class MultiTargetTrieNodeJsoConverter<TItem> : JsonConverter<MultiTargetTrieNode<TItem>>
    where TItem : notnull
{
    public override void WriteJson(JsonWriter writer, MultiTargetTrieNode<TItem>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        
        writer.WriteStartObject();
        writer.WritePropertyName("AlphabetSize");
        var alphabetSize = (int)(byte)value.GetType().GetField("_alphabetSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value);
        writer.WriteValue(alphabetSize);
        writer.WritePropertyName("Items");
        var items = value?.GetType().GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value)!;
        serializer.Serialize(writer, items);
        writer.WritePropertyName("Children");
        var children = value?.GetType().GetField("_children", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(value)!;
        serializer.Serialize(writer, children);
        writer.WriteEndObject();
    }

    public override MultiTargetTrieNode<TItem>? ReadJson(JsonReader reader, Type objectType, MultiTargetTrieNode<TItem>? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        
        reader.Read();
        reader.Read();
        var alphabetSize = reader.ReadAsInt32();
        reader.Read();
        reader.Read();
        var items = serializer.Deserialize<List<TItem>>(reader);
        reader.Read();
        reader.Read();
        var children = serializer.Deserialize<MultiTargetTrieNode<TItem>?[]>(reader);
        reader.Read();
        return new MultiTargetTrieNode<TItem>((byte)alphabetSize, items, children);
    }
}