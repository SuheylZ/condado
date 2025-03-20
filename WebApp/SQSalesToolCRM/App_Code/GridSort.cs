using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GridSort
/// </summary>
[Serializable()]
public class GridSortInfo
{
    public string Column;
    public bool Accending;

    public GridSortInfo(string archive)
    {
        FromString(archive);
    }
    public GridSortInfo(string column, bool IsAsc)
    {
        Column = column;
        Accending = IsAsc;
    }
    
    public bool IsValid
    {
        get
        {
            return Column != string.Empty;
        }
    }
    public override string ToString()
    {
        return Column + ":" + (Accending? "A" : "D");
    }
    private void FromString(string arg)
    {
        char[] splitter = new char[':'];
        string[] tmp = null; 
        try{
            tmp=arg.Split(splitter);
            Column = tmp[0];
            Accending = tmp[1] == "A";
        }catch{
            Column = string.Empty;
        }
    }
}
