using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

    //열의 순서대로 추가하는
	public static List<Dictionary<string, object>> Read(string file)
	{
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load(file) as TextAsset;

		var lines = Regex.Split(data.text, LINE_SPLIT_RE);

		if (lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++)
		{
			var values = Regex.Split(lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                object finalvalue = value;
				int n;
				float f;
				if (int.TryParse(value, out n))
				{
					finalvalue = n;
				}
				else if (float.TryParse(value, out f))
				{
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add(entry);
		}
		return list;
	}

    //열의 숫자대로 
    public static Dictionary<int,Dictionary<string, object>> ddRead( string file )
    {
        var list = new Dictionary<int, Dictionary<string, object>>();
        TextAsset data = Resources.Load( file ) as TextAsset;

        var lines = Regex.Split( data.text, LINE_SPLIT_RE );

        if(lines.Length <= 1) return list;

        var header = Regex.Split( lines[0], SPLIT_RE );
        for(var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split( lines[i], SPLIT_RE );
            if(values.Length == 0 || values[0] == "") continue;
            
            var entry = new Dictionary<string, object>();
            string firstvalue = values[0].TrimStart( TRIM_CHARS ).TrimEnd( TRIM_CHARS ).Replace( "\\", "" );
            int id;
            int finalIdx = 0;
            if(int.TryParse( firstvalue, out id ))
            {
                finalIdx = id;
            }

            for(var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart( TRIM_CHARS ).TrimEnd( TRIM_CHARS ).Replace( "\\", "" );
                object finalvalue = value;
                int n;
                float f;
                if(int.TryParse( value, out n ))
                {
                    finalvalue = n;
                }
                else if(float.TryParse( value, out f ))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }

            list[finalIdx] = entry ;
        }
        return list;
    }

    //첫 번째 밸류로 저장
    public static Dictionary<string,Dictionary<string, object>> ReadTest( string file )
    {
        var list = new Dictionary<string,Dictionary<string, object>>();
        TextAsset data = Resources.Load( file ) as TextAsset;

        var lines = Regex.Split( data.text, LINE_SPLIT_RE );

        if(lines.Length <= 1) return list;

        var header = Regex.Split( lines[0], SPLIT_RE );
        for(var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split( lines[i], SPLIT_RE );
            if(values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            string firstvalue = values[0];
            for(var j = 1; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart( TRIM_CHARS ).TrimEnd( TRIM_CHARS ).Replace( "\\", "" );
                object finalvalue = value;
                int n;
                float f;
                if(int.TryParse( value, out n ))
                {
                    finalvalue = n;
                }
                else if(float.TryParse( value, out f ))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list[firstvalue] = entry ;
        }
        return list;
    }

    
    //인덱스로만
  public static List<List<object>> Parsing( string file ) 
    { 
        var list = new List<List<object>>(); 
        TextAsset data = Resources.Load( file ) as TextAsset;
        var lines = Regex.Split( data.text, LINE_SPLIT_RE ); 
        if(lines.Length <= 1) 
            return list; var header = Regex.Split( lines[0], SPLIT_RE );
        for(var i = 1; i < lines.Length; i++) 
        { var values = Regex.Split( lines[i], SPLIT_RE );
            if(values.Length == 0 || values[0] == "") 
                continue; var entry = new List<object>();
            for(var j = 0; j < header.Length && j < values.Length; j++) 
            { 
                string value = values[j]; value = value.TrimStart( TRIM_CHARS ).TrimEnd( TRIM_CHARS ).Replace( "\\", "" );
                object finalvalue = value;
                int n; 
                float f; 
                if(int.TryParse( value, out n ))
                    finalvalue = n; 
                else if(float.TryParse( value, out f ))
                    finalvalue = f; entry[j] = finalvalue;

                entry.Add( finalvalue );
            }
            list.Add( entry ); 

        } return list; }
}