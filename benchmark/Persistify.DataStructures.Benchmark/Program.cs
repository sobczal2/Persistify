using System;
using Persistify.DataStructures.SuffixTree;

const string text = "ana are mere ana";
var suffixTree = new SuffixTree(text);

var results = suffixTree.Search("ana");
Console.WriteLine("Positions of 'ana': " + string.Join(", ", results));