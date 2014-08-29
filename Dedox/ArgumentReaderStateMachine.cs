﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dedox
{
    public class ArgumentReaderStateMachine
    {
        private IArgumentReaderState _state;

        private readonly List<FileInfo> _files = new List<FileInfo>(); 

        public ArgumentReaderStateMachine()
        {
            _state = new ArgumentReaderState(this);
        }

        public void Accept(string arg)
        {
            _state = _state.Accept(arg);
        }

        public bool Verbose { get; set; }

        public string OutputDirectory { get; set; }

        public void AddInputFile(string s)
        {
            var file = new FileInfo(s);
            if (!file.Exists)
            {
                throw new FileNotFoundException("No such input file. " + file.FullName);
            }

            _files.Add(file);
        }

        public List<FileInfo> GetInputFiles()
        {
            return _files.ToList();
        } 
    }
}
