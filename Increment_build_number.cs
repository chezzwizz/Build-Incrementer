using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;

namespace Knight_builds
{
    public class Increment_build_number
    {
        private enum Increment_target
        {
            major,
            minor,
            patch,
            build
        }

        
        public static void Main(string[] args)
        {
            if (args.Length > 2 || args.Length < 1) {
                Console.Error.WriteLine("Usage: incbuild <filename> [major|minor|patch|build]");
                return;
            }
            
            string filename = args[0];

            Increment_target increment_target = Increment_target.build;

            if (args.Length > 1)
            {
                string target = args[1];
                try
                {
                    Enum.TryParse<Increment_target>(target, out increment_target);
                }
                catch (ArgumentException argex)
                {
                    Console.Error.WriteLine("Target `{0}` is not recognized. Using `build` number as the increment target. Target must be one of { \"major\", \"minor\", \"patch\", \"build\" }");
                    increment_target = Increment_target.build;
                }
            }


            TextReader read_stream;
            TextWriter write_stream;

            try
            {
                read_stream = File.OpenText(filename);
            }
            catch (FileNotFoundException fnfex)
            {
                Console.Error.WriteLine(fnfex.Message);
                return;
            }

            string xml_string = read_stream.ReadToEnd();

            int start_index = xml_string.IndexOf("<ProductVersion>") + "<ProductVersion>".Length;
            int end_index = xml_string.IndexOf("</ProductVersion>", start_index);

            //Console.WriteLine("start_index: {0}, end_index: {1}", start_index, end_index);

            string version_string = xml_string.Substring(start_index, end_index - start_index);
            string[] version_info = version_string.Split('.');

            Console.WriteLine("Incrementing `{0}` number. Found {1} total versioning numbers.", Enum.GetName(typeof(Increment_target), increment_target), version_info.Length);

            int increment_target_index;

            if (increment_target == Increment_target.build
                && ((int)Increment_target.build < version_info.Length - 1
                    || (int)Increment_target.build > version_info.Length - 1))
            {
                increment_target_index = version_info.Length - 1;
            }
            else
            {
                increment_target_index = (int)increment_target;
            }

            Console.WriteLine("Version info:");

            for (int i = 0; i < version_info.Length; ++i)
            {
                if (version_info.Length - 1 < (int)Increment_target.build && i == version_info.Length - 1)
                {
                    Console.WriteLine("\t{0} ({1})", version_info[i], Enum.GetName(typeof(Increment_target), Increment_target.build));
                }
                else
                {
                    Console.WriteLine("\t{0} ({1})", version_info[i], Enum.GetName(typeof(Increment_target), i));
                }
            }

            Console.Write("Incrementing `{0}` from {1} to ", Enum.GetName(typeof(Increment_target), increment_target), version_info[increment_target_index]);

            int build_number = int.Parse(version_info[increment_target_index]);

            Console.WriteLine("{0}.", ++build_number);


            version_info[increment_target_index] = build_number.ToString();

            if (increment_target == Increment_target.major || increment_target == Increment_target.minor)
            {
                for (int k = (int)increment_target_index + 1; k < version_info.Length - 1; ++k)
                {
                    version_info[k] = 0.ToString();
                }
            }

            StringBuilder version_builder = new StringBuilder();

            int j = 1;

            while (j < version_info.Length)
            {
                version_builder.Append(string.Format("{0}.", version_info[j - 1]));
                ++j;
            }

            version_builder.Append(version_info[j - 1]);

            Console.WriteLine("New version info:");
            for (int i = 0; i < version_info.Length; ++i)
            {
                if (version_info.Length - 1 < (int)Increment_target.build && i == version_info.Length - 1)
                {
                    Console.WriteLine("\t{0} ({1})", version_info[i], Enum.GetName(typeof(Increment_target), Increment_target.build));
                }
                else
                {
                    Console.WriteLine("\t{0} ({1})", version_info[i], Enum.GetName(typeof(Increment_target), i));
                }
            }

            xml_string = xml_string.Remove(start_index, end_index - start_index);
            xml_string = xml_string.Insert(start_index, version_builder.ToString());

            read_stream.Close();
            write_stream = new StreamWriter(filename, false);

            write_stream.Write(xml_string);
            write_stream.Close();
        }

        private static int read_to(TextReader reader, char c)
        {
            int total_read = 0;

            while ((char)reader.Read() != c)
                total_read++;

            return total_read;
        }
    }
}