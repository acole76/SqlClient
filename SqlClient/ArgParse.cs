using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ArgumentParser
{
  public class ArgParse
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();
    List<ArgItem> argList = new List<ArgItem>();

    public enum ArgParseType
    {
      Long = 0,
      Int = 1,
      DateTime = 2,
      String = 3,
      Boolean = 4,
      File = 5,
      Pid = 6 //process id
    };

    public ArgParse(params ArgItem[] _argClass)
    {
      foreach (ArgItem arg in _argClass)
      {
        argList.Add(arg);
      }
    }

    public ArgParse(List<ArgItem> _argList)
    {
      argList = _argList;
    }

    private ArgItem getArgItemBySwitch(string arg)
    {
      if (arg.StartsWith("--"))
      {
        foreach (ArgItem item in argList)
        {
          if (item.longName == arg.Substring(2))
          {
            return item;
          }
        }
      }
      else
      {
        foreach (ArgItem item in argList)
        {
          if (item.shortName == arg.Substring(1))
          {
            return item;
          }
        }
      }

      return null;
    }

    public void parse(string[] args)
    {
      string prev = null;
      foreach (string arg in args)
      {
        if (arg == "-h" || arg == "--help")
        {
          showHelp("");
          return;
        }

        if (arg.StartsWith("--") || arg.StartsWith("-"))
        {
          ArgItem item = getArgItemBySwitch(arg);

          if (item == null)
          {
            showHelp(string.Format("Unknown switch: {0}", arg));
            return;
          }

          prev = item.longName;
          if(dict.ContainsKey(item.longName))
          {
            showHelp(string.Format("duplicate switch: {0}", item.longName));
            return;
          }

          dict.Add(item.longName, item.defaultValue);
        }
        else
        {
          if (prev != null)
          {
            dict[prev] = arg;
          }
        }
      }

      // check is required
      foreach (ArgItem argClass in argList)
      {
        if (argClass.required)
        {
          if (!dict.ContainsKey(argClass.longName))
          {
            showHelp(string.Format("Field {0} is required but is missing.", argClass.longName));
            return;
          }
        }
      }

      foreach (ArgItem argClass in argList)
      {

      }

      // check is valid type
      foreach (ArgItem argClass in argList)
      {
        try
        {
          string dataType = argClass.dataType.ToString();
          switch (argClass.argType)
          {
            case ArgParseType.Int:
              Int32.Parse(dict[argClass.longName]);
              break;
            case ArgParseType.Long:
              Int64.Parse(dict[argClass.longName]);
              break;
            case ArgParseType.DateTime:
              DateTime.Parse(dict[argClass.longName]);
              break;
            case ArgParseType.Boolean:
              bool.Parse(dict[argClass.longName]);
              break;
            case ArgParseType.File:
              if (!File.Exists(dict[argClass.longName]))
              {
                throw new Exception("File does not exist");
              }
              break;
            case ArgParseType.Pid:
              Int32 pid = Int32.Parse(dict[argClass.longName]);
              Process proc = Process.GetProcessById(pid);
              if (proc == null)
              {
                throw new Exception("Process does not exist");
              }
              break;
            default:
              break;
          }
        }
        catch (Exception e)
        {
          showHelp(string.Format("{0} - {1}", argClass.longName, e.Message));
          break;
        }
      }
    }

    public void showHelp(string msg)
    {
      bool hasOptional = false;
      if (msg.Length > 0)
      {
        Console.WriteLine("Error: {0}\n", msg);
      }

      Console.WriteLine("Usage: {0} <options>", AppDomain.CurrentDomain.FriendlyName);
      Console.WriteLine("\tRequired");
      foreach (ArgItem argClass in argList)
      {
        if (argClass.required == true)
        {
          Console.WriteLine("\t\t-{0}, --{1}\t{2}", argClass.shortName, argClass.longName, argClass.helpText);
        }
        else
        {
          hasOptional = true;
        }
      }

      if (hasOptional)
      {
        Console.WriteLine("\n\tOptional");
        foreach (ArgItem argClass in argList)
        {
          if (argClass.required == false)
          {
            Console.WriteLine("\t\t-{0}, --{1}\t{2}", argClass.shortName, argClass.longName, argClass.helpText);
          }
        }
      }

      if(Debugger.IsAttached)
      {
        Console.ReadLine();
      }

      Environment.Exit(0);
    }

    public T Get<T>(string key)
    {
      if (!dict.ContainsKey(key))
        return default(T);

      try
      {
        return (T)Convert.ChangeType(dict[key], typeof(T));
      }
      catch (Exception)
      {
        return default(T);
      }
    }
  }

  public class ArgItem
  {
    public string longName { get; set; } //double dash option --domain
    public string shortName { get; set; } //single dash option -d
    public string helpText { get; set; }
    public string defaultValue { get; set; }
    public bool required { get; set; }
    public Type dataType { get; set; }
    public ArgParse.ArgParseType argType { get; set; }

    public ArgItem(string _longName, string _shortName, bool _required)
    {
      init(_longName, _shortName, _required, "", "", ArgParse.ArgParseType.String);
    }

    public ArgItem(string _longName, string _shortName, bool _required, string _helpText)
    {
      init(_longName, _shortName, _required, _helpText, "", ArgParse.ArgParseType.String);
    }
    public ArgItem(string _longName, string _shortName, bool _required, string _helpText, string _defaultValue)
    {
      init(_longName, _shortName, _required, _helpText, _defaultValue, ArgParse.ArgParseType.String);
    }

    public ArgItem(string _longName, string _shortName, bool _required, ArgParse.ArgParseType _dataType)
    {
      init(_longName, _shortName, _required, "", "", _dataType);
    }

    public ArgItem(string _longName, string _shortName, bool _required, string _helpText, ArgParse.ArgParseType _dataType)
    {
      init(_longName, _shortName, _required, _helpText, "", _dataType);
    }

    public ArgItem(string _longName, string _shortName, bool _required, string _helpText, string _defaultValue, ArgParse.ArgParseType _dataType)
    {
      init(_longName, _shortName, _required, _helpText, _defaultValue, _dataType);
    }

    private void init(string _longName, string _shortName, bool _required, string _helpText, string _defaultValue, ArgParse.ArgParseType _dataType)
    {
      longName = _longName;
      shortName = _shortName;
      helpText = _helpText;
      defaultValue = _defaultValue;
      required = _required;
      argType = _dataType;
      dataType = _dataType.ToType();
    }
  }

  public static class ArgParseExtensions
  {
    public static Type ToType(this ArgParse.ArgParseType argType)
    {
      switch (argType)
      {
        case ArgParse.ArgParseType.DateTime:
          return DateTime.MaxValue.GetType();
        case ArgParse.ArgParseType.Int:
          return int.MaxValue.GetType();
        case ArgParse.ArgParseType.Long:
          return long.MaxValue.GetType();
        case ArgParse.ArgParseType.Boolean:
          return true.GetType();
        default:
          return "".GetType();
      }
    }
  }
}

