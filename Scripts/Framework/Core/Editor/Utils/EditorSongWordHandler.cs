using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class EditorSongWordHandler
{
	#region [ --- ] Property
	string path;

	List<string> allWords = new List<string> ();
	List<int> cachedID = new List<int> ();

	string others = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM" +
		"`1234567890-= ~!@#$%^&*()-+[]\\;'',./{}|:\"\"<>?？！，。āáǎàōóǒòēéěèīíǐìūúǔùǖǘǚǜü";
	#endregion





	public void GetAndSave ()
	{
		path = Application.dataPath + "/Fonts/allWords.txt";
		Debug.Log (path);
		StringBuilder sb = new StringBuilder (2048);

		sb.Append (others + "\n");
		var assets = Resources.LoadAll<TextAsset> ("Songs");
		foreach (var asset in assets) {
			string text = asset.text.Trim ();
			for (int i = 0; i < text.Length; i++) {
				int index = (int)text[i];
				if (index > 127 && !cachedID.Contains (index)) {
					cachedID.Add (index);
					sb.Append (text[i]);
				}
			}
			sb.Append ("\n");
		}
		File.WriteAllText (path, sb.ToString ());
		AssetDatabase.Refresh ();
	}

}
//EditorSongWordHandler














