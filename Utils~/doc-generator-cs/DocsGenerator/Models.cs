using System.Text.Json.Serialization;
using System.Xml;

namespace DocsGenerator
{
	public class Page
	{
		public string Title { get; set; } = "API Reference";
		public List<Cls> Content { get; set; } = [];
	}

	public class UnityManifest
	{
		[JsonPropertyName("version")]
		public string Version { get; set; }
	}

	public class Meta
	{
		public string Version { get; init; }
		public string Package { get; init; }
		public string Lang { get; init; }
		public string Name { get; init; }
	}

	public class Docs(XmlDocument xml)
	{
		private readonly XmlDocument xml = xml;

		public List<Cls> Classes
		{
			get
			{
				XmlElement element = xml.DocumentElement ?? throw new Exception("Invalid document");
				XmlNodeList list = element.SelectNodes("compounddef[@kind='class']") ??
				                   throw new Exception("Invalid document");
				List<Cls> classes = [];
				foreach (XmlNode node in list)
				{
					classes.Add(new Cls(node));
				}

				return classes;
			}
		}
	}

	public class Cls(XmlNode node)
	{
		private readonly XmlNode node = node;

		public string Type
		{
			get => (Fields.Count > 0 || Methods != null && Methods.Count == 0) ? "type" : "class";
		}

		public string Name
		{
			get => node.SelectSingleNode("compoundname")?.InnerText?.Replace("::", ".") ??
			       throw new Exception("Missing name of class");
		}

		public string Description
		{
			get => node.SelectSingleNode("briefdescription")?.InnerText ?? "";
		}

		public string? Snippet
		{
			get
			{
				if (Type.Equals("type"))
				{
					string res = "public class " + Name.Replace("::", ".");
					var template = node.SelectSingleNode("templateparamlist");
					if (template != null)
					{
						res += "<" + template.InnerText + ">";
					}

					var baseClass = node.SelectSingleNode("basecompoundref");
					if (baseClass != null)
					{
						res += " : " + baseClass.InnerText;
					}

					res += "\n{";
					foreach (Field field in Fields)
					{
						res += "\n    " + field.Type.Name + " " + field.Name + " {get; set;}";
					}

					res += "\n}";
					return res;
				}

				return null;
			}
		}

		public List<Field> Fields
		{
			get
			{
				XmlNodeList? list = node.SelectNodes("sectiondef/memberdef[@kind='property'][@prot='public']");
				List<Field> fields = [];
				if (list != null)
				{
					foreach (XmlNode node in list)
					{
						fields.Add(new Field(node));
					}
				}

				return fields;
			}
		}

		public List<Method>? Methods
		{
			get
			{
				XmlNodeList? list = node.SelectNodes("sectiondef/memberdef[@kind='function'][@prot='public']");
				List<Method> methods = [];
				if (list != null)
				{
					foreach (XmlNode node in list)
					{
						methods.Add(new Method(node));
					}
				}

				if (methods.Count == 0 && Fields.Count > 0) return null;
				return methods;
			}
		}
	}

	public class Field(XmlNode node)
	{
		private readonly XmlNode node = node;

		public string Name
		{
			get => node.SelectSingleNode("name")?.InnerText ?? throw new Exception("Missing name of field");
		}

		public string Description
		{
			get => node.SelectSingleNode("briefdescription")?.InnerText ?? "";
		}

		public Type Type
		{
			get => new(node);
		}
	}

	public class Method(XmlNode node)
	{
		private readonly XmlNode node = node;

		public string Type
		{
			get => "method";
		}

		public string Name
		{
			get => node.SelectSingleNode("name")?.InnerText ?? throw new Exception("Missing name of method");
		}

		public string Description
		{
			get => node.SelectSingleNode("briefdescription")?.InnerText ?? "";
		}

		public string Snippet
		{
			get => (node.SelectSingleNode("definition")?.InnerText ?? "") +
			       (node.SelectSingleNode("argsstring")?.InnerText ?? "");
		}

		public string MethodType
		{
			get => node.Attributes["static"].Value.Equals("no") ? "method" : "static";
		}

		public List<Param> Params
		{
			get
			{
				XmlNodeList? list = node.SelectNodes("param");
				List<Param> parms = [];
				if (list != null)
				{
					foreach (XmlNode node2 in list)
					{
						parms.Add(new Param(node, node2));
					}
				}

				return parms;
			}
		}

		public List<Return>? Returns
		{
			get
			{
				var item = node.SelectSingleNode("detaileddescription/para/simplesect");
				var ret = new Return(node, item);
				if (!ret.Type.Name.Equals("void") && !ret.Type.Name.Equals(""))
				{
					List<Return> list = [new Return(node, item)];
					return list;
				}

				return null;
			}
		}
	}

	public class Param(XmlNode method, XmlNode node)
	{
		private readonly XmlNode method = method;
		private readonly XmlNode node = node;

		public string Name
		{
			get => node.SelectSingleNode("declname")?.InnerText ?? throw new Exception("Missing name of param");
		}

		public string Description
		{
			get => method
				?.SelectSingleNode(
					"detaileddescription/para/parameterlist/parameteritem[parameternamelist/parametername='" + Name +
					"']/parameterdescription/para")?.InnerText ?? "";
		}

		public Type Type
		{
			get => new(node);
		}
	}

	public class Return(XmlNode method, XmlNode? node)
	{
		private readonly XmlNode method = method;
		private readonly XmlNode? node = node;

		public Type Type
		{
			get => new(method);
		}

		public string Description
		{
			get => node?.InnerText ?? "";
		}
	}

	public class Type(XmlNode node)
	{
		private readonly XmlNode node = node;

		public string Name
		{
			get => node.SelectSingleNode("type")?.InnerText ?? throw new Exception("Missing name of type");
		}

		public bool Optional
		{
			get => node.SelectSingleNode("defval") != null;
		}
	}
}