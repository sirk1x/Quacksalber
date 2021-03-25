using System;

namespace QuickConvert
{
    class Program
    {
        static string[] _files;

        static string MainPath => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        static string InPath = MainPath + "\\sounds\\";
        static string OutPath = MainPath + "\\sounds\\converted\\";

        static void Main(string[] args)
        {           
            //load sounds from our directory
            GetSounds();
            //create the directory if it doesnt exist
            CreatePath();
            //check if we have any files
            if(_files.Length == 0)
            {
                //exit if we dont
                Console.WriteLine("No files found!");
                Console.ReadKey();
                return;
            }
            //loop through our file array
            foreach (var item in _files)
            {
                //extract our filename and append with conv.mp3
                var _f = GetFileName(item) + "conv.mp3";
                //check if the file exists, if it doesnt we want to convert.
                if(!System.IO.File.Exists(OutPath + _f))
                {
                    //start our converter
                    var converter = Create(item, OutPath + _f, "lame.exe");
                    //wait for it to exit
                    while (!converter.HasExited) System.Threading.Thread.Sleep(16);
                }

                //create our final filename
                var _cut = GetFileName(item) + ".mp3";
                //start the cutting process.
                var _pcs = Cut(OutPath + _f, OutPath + _cut, "ffmpeg.exe");
                //wait for ffmpeg to exit
                while (!_pcs.HasExited) System.Threading.Thread.Sleep(16);
                //delete the old file
                Delete(OutPath +_f);
                //continue
            }
        }

        /// <summary>
        /// delete a file if it exists.
        /// </summary>
        /// <param name="_fullFilePath"></param>
        static void Delete(string _fullFilePath)
        {
            if (System.IO.File.Exists(_fullFilePath))
                System.IO.File.Delete(_fullFilePath);
        }

        /// <summary>
        /// create the directory for our converted sounds
        /// </summary>
        static void CreatePath()
        {
            if (!System.IO.Directory.Exists(OutPath))
                System.IO.Directory.CreateDirectory(OutPath);
        }

        /// <summary>
        /// loading all sounds from the sound directory or creating the directory if it doesnt exist.
        /// </summary>
        static void GetSounds()
        {
            if (!System.IO.Directory.Exists(InPath))
                System.IO.Directory.CreateDirectory(InPath);
            _files = System.IO.Directory.GetFiles(InPath);
        }

        /// <summary>
        /// Extract the filename from a given filepath without extension
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string GetFileName(string input)
        {
            var _itrimPath = input.LastIndexOf('\\') + 1;
            var _fileName = input.Substring(_itrimPath);
            var _itrimEnd = _fileName.LastIndexOf('.');
            return _fileName.Remove(_itrimEnd, _fileName.Length - _itrimEnd);
        }

        /// <summary>
        /// extract directory path from a filepath
        /// </summary>
        /// <param name="_fullFilePath"></param>
        /// <returns></returns>
        static string GetPath(string _fullFilePath)
        {
            var _indexPoint = _fullFilePath.LastIndexOf('\\') + 1;
            return _fullFilePath.Remove(_indexPoint, _fullFilePath.Length - _indexPoint);
        }

        /// <summary>
        /// gets the extension
        /// </summary>
        /// <param name="_input"></param>
        /// <returns></returns>
        static string GetExtension(string _input)
        {
            return _input.Substring(_input.LastIndexOf('.'));
        }

        /// <summary>
        /// starts the lame encoder and instructs to resample to 44100 khz
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outFileName"></param>
        /// <returns></returns>
        private static System.Diagnostics.Process Create(string inputFile, string outFileName, string processName)
        {       
            return System.Diagnostics.Process.Start(MainPath + "\\" + processName, "-b 320 -q 0 --resample 44.1 " + inputFile + " " + outFileName);
        }

        /// <summary>
        /// starts ffmpeg with instructions to remove any metadata, copy the previously applied codec and trim the length to 1 minute
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outFileName"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        private static System.Diagnostics.Process Cut(string inputFile, string outFileName, string processName)
        {
            return System.Diagnostics.Process.Start(MainPath + "\\" + processName, string.Format("-i {0} -vn -map_metadata -1 -acodec copy -ss 00:00:00 -t 00:01:00 {1}", inputFile, outFileName));
        }
    }
}
