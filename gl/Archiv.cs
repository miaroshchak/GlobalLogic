using System;
using System.Collections.Generic;

[Serializable]
public class Archiv
{
    public string path { get; set; }
    public List<string> DirNameArchiv { get; set; }
    public List<FileIN> FileArchiv { get; set; }
}