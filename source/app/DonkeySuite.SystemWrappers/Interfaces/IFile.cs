﻿using System.IO;

namespace DonkeySuite.SystemWrappers.Interfaces
{
    public interface IFile
    {
        bool Exists(string path);
        FileStream Open(string path, FileMode mode);
        byte[] ReadAllBytes(string path);
        void Move(string oldPath, string newPath);
    }
}