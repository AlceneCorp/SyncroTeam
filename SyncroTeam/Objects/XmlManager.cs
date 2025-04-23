using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SyncroTeam.Objects
{
    public static class XmlManager
    {
        /// <summary>
        /// Sérialise un objet en XML et le sauvegarde dans un fichier
        /// </summary>
        public static void SaveObject(object obj, string filePath)
        {
            if(obj != null)
            {
                XElement root = SerializeObject(obj, obj.GetType().Name);
                root.Save(filePath);
            }
        }

        /// <summary>
        /// Charge un objet depuis un fichier XML
        /// </summary>
        public static T LoadObject<T>(string filePath) where T : new()
        {
            XElement root = XElement.Load(filePath);
            return (T)DeserializeObject(root, typeof(T));
        }

        /// <summary>
        /// Convertit un objet en un élément XML
        /// </summary>
        private static XElement SerializeObject(object obj, string name)
        {
            if(obj == null)
                return new XElement(name);

            Type type = obj.GetType();
            XElement element = new XElement(name);

            foreach(PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if(prop.GetIndexParameters().Length > 0)
                    continue;

                // Sauter les propriétés avec [XmlIgnore]
                if(Attribute.IsDefined(prop, typeof(XmlIgnoreAttribute)))
                {
                    continue;
                }

                object value = prop.GetValue(obj);
                if(value == null) continue;

                XElement child;

                // Types simples (string, bool, int, double...)
                if(value is string || prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(decimal))
                {
                    child = new XElement(prop.Name, value);
                }
                // Enum
                else if(prop.PropertyType.IsEnum || Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum == true)
                {
                    child = new XElement(prop.Name, value.ToString());
                }
                // Dates et temps
                else if(prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateOnly) || prop.PropertyType == typeof(TimeOnly))
                {
                    child = new XElement(prop.Name, value.ToString());
                }
                // Couleurs (converties en int ARGB)
                else if(prop.PropertyType == typeof(Color))
                {
                    child = new XElement(prop.Name, ((Color)value).ToArgb());
                }
                // Dictionnaires génériques
                else if(typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                {
                    child = new XElement(prop.Name);
                    IDictionary dict = (IDictionary)value;

                    foreach(var key in dict.Keys)
                    {
                        XElement item = new XElement("Item");
                        item.Add(new XElement("Key", key.ToString()));
                        item.Add(SerializeObject(dict[key], "Value"));
                        child.Add(item);
                    }
                }
                // Listes génériques
                else if(typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    child = new XElement(prop.Name);

                    foreach(var item in (IEnumerable)value)
                    {
                        if(item is string str)
                        {
                            child.Add(new XElement("string", str));
                        }
                        else if(item is Enum en)
                        {
                            child.Add(new XElement(item.GetType().Name, en.ToString()));
                        }
                        else
                        {
                            child.Add(SerializeObject(item, item.GetType().Name));
                        }
                    }
                }
                // Types personnalisés récursifs
                else
                {
                    child = SerializeObject(value, prop.Name);
                }

                element.Add(child);
            }

            return element;
        }

        /// <summary>
        /// Convertit un élément XML en objet C#
        /// </summary>
        private static object DeserializeObject(XElement element, Type type)
        {
            object instance = Activator.CreateInstance(type);

            foreach(PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                XElement child = element.Element(prop.Name);
                if(child == null) continue;

                Type propType = prop.PropertyType;

                try
                {
                    // Chaînes de caractères
                    if(propType == typeof(string))
                    {
                        prop.SetValue(instance, child.Value);
                    }
                    // Types primitifs (bool, int, double...)
                    else if(propType.IsPrimitive || propType == typeof(decimal))
                    {
                        prop.SetValue(instance, Convert.ChangeType(child.Value, propType));
                    }
                    // Enumérations
                    else if(propType.IsEnum)
                    {
                        prop.SetValue(instance, Enum.Parse(propType, child.Value));
                    }
                    // Dates et temps
                    else if(propType == typeof(DateTime))
                    {
                        prop.SetValue(instance, DateTime.Parse(child.Value));
                    }
                    else if(propType == typeof(DateOnly))
                    {
                        prop.SetValue(instance, DateOnly.Parse(child.Value));
                    }
                    else if(propType == typeof(TimeOnly))
                    {
                        prop.SetValue(instance, TimeOnly.Parse(child.Value));
                    }
                    // Couleurs
                    else if(propType == typeof(Color))
                    {
                        prop.SetValue(instance, Color.FromArgb(int.Parse(child.Value)));
                    }
                    // Dictionnaires
                    else if(typeof(IDictionary).IsAssignableFrom(propType))
                    {
                        var dict = (IDictionary)Activator.CreateInstance(propType);
                        Type[] args = propType.GetGenericArguments();
                        Type keyType = args[0];
                        Type valType = args[1];

                        foreach(XElement item in child.Elements("Item"))
                        {
                            object key = Convert.ChangeType(item.Element("Key").Value, keyType);
                            XElement valElement = item.Element("Value").Elements().First();
                            object value = DeserializeObject(valElement, valType);
                            dict.Add(key, value);
                        }

                        prop.SetValue(instance, dict);
                    }
                    // Listes génériques
                    else if(typeof(IList).IsAssignableFrom(propType))
                    {
                        var list = (IList)Activator.CreateInstance(propType);
                        Type itemType = propType.GetGenericArguments()[0];

                        foreach(XElement item in child.Elements())
                        {
                            list.Add(DeserializeObject(item, itemType));
                        }

                        prop.SetValue(instance, list);
                    }
                    // Autres objets personnalisés
                    else
                    {
                        object subObj = DeserializeObject(child, propType);
                        prop.SetValue(instance, subObj);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Erreur désérialisation {prop.Name} : {ex.Message}");
                }
            }

            return instance;
        }
    }
}
