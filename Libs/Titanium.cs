using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

//Namespace contains library classes containing add-on to basic c# tools
namespace Titanium {
	/// <summary>
	/// Just my library of small functions that makes c# programming easier. The automatization of automatization instrument
	/// <para> Despite of the program license, THIS file is <see href="https://creativecommons.org/licenses/by-nc-sa/4.0">CC BY-NC-SA</see></para>
	/// <list type="table">
	/// <item>
	/// <term>Author</term>
	/// <see href="https://github.com/TuTAH1">Титан</see>
	/// </item>
	/// </list>
	/// </summary>   
	public static class TypesFuncs { //!17.06.2023 SplitN func


		#region Parsing

			#region IsType
			public static bool IsDigit(this char c) => c >= '0' && c <= '9';

			public static bool IsDoubleT(this char c) => (c >= '0' && c <= '9')||( c == '.' || c == ','||c=='-');
			#endregion

			#region ToType
				#region Int

				
					/// <summary>
					/// Преобразует число в char в Int
					/// </summary>
					/// <param name="str"></param>
					/// <returns>Число, если содержимое Char – число:
					/// -1 в противном случае</returns>
					public static int ToIntT(this char ch) =>
						ch is >= '0' and <= '9'?
							ch-'0' : -1;

					/// <summary>
					/// "534 Убирает все лишние символы и читает Int. 04" => 53404. Максимально оптимизирован, даже лучше стандартного
					/// </summary>
					/// <param name="str"></param>
					/// <param name="CanBeNegative">Может ли число быть отрицательным (если нет, то - будет игнорироваться, иначе он будет учитываться только если стоит рядом с числом)</param>
					/// <param name="StopIfNumberEnded">If true, stops parsing if int already found, but current symbol isn't a number</param>
					/// <param name="ExceptionValue">Возвращаемое значение при исключении (если не указывается, то при исключении... вызывается исключение</param>
					/// <returns>Числа, если они содержаться в строке</returns>
					public static int ToIntT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, int? ExceptionValue = null) //:16.05.2022 removed reduntant ischar check
					{
						int Number = 0;
						bool isContainsNumber = false;
						bool negative = false;
						foreach (var ch in str)
						{
							if (CanBeNegative&&!isContainsNumber) //:multiple ifs each cycle isn't affects perfomance; CanBeNegative не проверялось
							{
								if (ch == '-') negative = true; 
								else if (ch != ' ') negative = false;
							}
							if (ch.IsDigit())
							{
								isContainsNumber = true;
								if (Number > int.MaxValue / 10) return negative ? int.MinValue : int.MaxValue; //:none or positive perfomance effect
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Number = Number * 10 + (ch - '0'); 
							} else if(StopIfNumberEnded && isContainsNumber) break;
						}

						if (!isContainsNumber)
						{
							if (ExceptionValue == null)
							{
								throw new ArgumentException("Строка не содержит числа");
							}
							else return (int)ExceptionValue;
						}
						
						return negative? -Number:Number;
					}

					public static long ToLongT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, long? ExceptionValue = null)
					{
						long Number = 0;
						bool isContainsNumber = false;
						bool negative = false;
						foreach (var ch in str)
						{
							if (CanBeNegative&&!isContainsNumber) //:multiple ifs each cycle isn't affects perfomance; CanBeNegative не проверялось
							{
								if (ch == '-') negative = true; 
								else if (ch != ' ') negative = false;
							}
							if (ch.IsDigit())
							{
								isContainsNumber = true;
								if (Number > long.MaxValue / 10) return negative ? long.MinValue : long.MaxValue; //:none or positive perfomance effect
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Number = Number * 10 + (ch - '0'); 
							} else if(StopIfNumberEnded && isContainsNumber) break;
						}

						if (!isContainsNumber)
						{
							if (ExceptionValue == null)
							{
								throw new ArgumentException("Строка не содержит числа");
							}
							else return (long)ExceptionValue;
						}
						
						return negative? -Number:Number;
					}


					public static int? ToNIntT(this string str, bool CanBeNegative = true,  bool StopIfNumberEnded = false) //:08.10.2021 new func
					{
						try
						{
							return ToIntT(str, CanBeNegative,StopIfNumberEnded);
						}
						catch (Exception)
						{
							return null;
						}
					}

		
					/// <summary>
					/// Searches for INT in string. Throws IndexOutOfRangeException if no any int found in this string.
					/// </summary>
					/// <param name="str"></param>
					/// <param name="thousandSeparator"></param>
					/// <returns>first INT number found in string</returns>
					public static int FindInt(this string str, char thousandSeparator = ' ') //! legacy Obsolete
					{
						int end = 0, start = 0;
						bool minus = false;
						while (!str[start].IsDigit())
						{
							minus = str[start] == '-';
							start++;
						}

						end = minus ? start - 1 : start;

						do
						{
							end++;
						} while ((str[end].IsDigit() || str[end] == thousandSeparator) && end < str.Length - 1);

						return Convert.ToInt32(str.Substring(start,end-start).Replace(thousandSeparator.ToString(), ""), new CultureInfo("en-US", true));
					}
					public static int FindIntBetween(this string MainStr, string LeftStr, string RightStr = null)
					{
						if (RightStr == null) RightStr = LeftStr;
						int right = MainStr.IndexOf(RightStr),
							left = MainStr.LastIndexOf(LeftStr, 0, right - 1);
						int number;

						do
						{ //TODO: Сначала искать IndexOf Right, потом LastIndexOf Left, 
							if (int.TryParse(MainStr.Substring(left+1,right-(left+1)), out number)) return number;
							right = MainStr.IndexOf(RightStr);
							left = MainStr.LastIndexOf(LeftStr, left, (right - left) - 1);
						} while (left > 0 && right > 0);

						return -1;
					}

					#endregion

				#region String

				//!17.06.2023

				/// <summary>
				/// Int to subscript numbers string
				/// </summary>
				/// <param name="number"></param>
				/// <returns></returns>
				public static string ToIndex(this int number)
				{
					string num = number.ToString();
					string index = "";

					for (int i = 0; i < num.Length; i++) {
						if (num[i] == '1') index += '₁';
						if (num[i] == '2') index += '₂';
						if (num[i] == '3') index += '₃';
						if (num[i] == '4') index += '₄';
						if (num[i] == '5') index += '₅';
						if (num[i] == '6') index += '₆';
						if (num[i] == '7') index += '₇';
						if (num[i] == '8') index += '₈';
						if (num[i] == '9') index += '₉';
						if (num[i] == '0') index += '₀';
					}
					return index;
				}

				public static string ToVisibleString(this string s) //!03.10.2021
				{
					string a = "";
					foreach (char c in s)
					{
						if (!char.IsControl(c)) a += c;
					}

					return a;
				}
				

				/// <summary>
				/// Преобразовывает string в double, обрезая дробную часть до момента, когда начинаются нули
				/// </summary>
				/// <param name="Double"></param>
				/// <param name="Accuracy"></param>
				/// <returns></returns>
				public static string ToStringT(this double Double, int Accuracy = 3) //:10.10.2021 Created
				{
					var String = Double.ToString(CultureInfo.InvariantCulture); //: Пока я не умею преобразовывать double в String с минимально возможными затратами самостоятельно
					var temp = String.Split('.');
					if (temp.Length == 1)
					{
						return temp[0];
					}
					else
					{
						string intPart = temp[0], decPart = temp[1];
						int superfluity = decPart.IndexOf('0'.Multiply(Accuracy)); //TODO: вместо нулей можно пробегать по строке, пока символ не начнет повторятся несколько раз
						if (superfluity == -1) return String;

						decPart = decPart.Slice(0, superfluity);
						return intPart + "." + decPart;
					}

				}

				public static string ToStringT<T>(this IEnumerable<T> Array, string Separator = "") //:21.04.2022 Added separator parametr, replaced [foreach] by [for]
				{
					string s = "";
					var Count = Array.Count();
					for (int i = 0; i < Count; i++)
					{
						s += Array.ElementAt(i);
						if (i != Count - 1) s += Separator;
					}

					return s;
				}

				public static string ToStringT<TKey, TValue> (this IDictionary<TKey, TValue> Dictionary, bool Vertical = true, string ItemSeparator = default, string PairSeparator = "\n", string BeforePair = null, string AfterPair = null)
				{
					ItemSeparator??= Vertical? " = " : ", ";

					if(Vertical) return BeforePair + string.Join(PairSeparator, Dictionary.Select(kv => kv.Key + ItemSeparator + kv.Value).ToArray()) + AfterPair;
					else
					{
						string result = BeforePair;
						foreach (var key in Dictionary.Keys)
						{
							result += key + ItemSeparator;
						}

						result = result.Replace(-ItemSeparator.Length,null,AfterPair + PairSeparator + BeforePair);
						
						foreach (var value in Dictionary.Values)
						{
							result += value + ItemSeparator;
						}

						return result + AfterPair;
					}

				}


					#region Bytes

				/// <summary>
				/// Reads String and parses to Short until meets a letter
				/// </summary>
				/// <param name="str"></param>
				/// <returns></returns>

				public static string ToHex(this byte[] bytes)
				{
					char[] c = new char[bytes.Length * 2];

					byte b;

					for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
					{
						b = ((byte)(bytes[bx] >> 4));
						c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

						b = ((byte)(bytes[bx] & 0x0F));
						c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
					}

					return new string(c);
				}

				#endregion

				#endregion

				#region Double

				/// <summary>
				/// "534 Убирает все лишние символы и читает double. 0.4" => 5340.4
				/// </summary>
				/// <param name="str"></param>
				/// <param name="CanBeNegative"></param>
				/// <param name="StopIfNumberEnded"></param>
				/// <param name="CanBeShortcuted">Может ли нуль целой части быть опущен (".23" вместо "0.23")</param>
				/// <param name="DotShouldBeAttachedToNumber"></param>
				/// <param name="Separator">Разделитель целой и дробной части</param>
				/// <param name="ExceptionValue"></param>
				/// <returns></returns>
				public static double ToDoubleT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, bool CanBeShortcuted = true, bool DotShouldBeAttachedToNumber = true, char Separator = '.', double? ExceptionValue = Double.NaN) //:23.05.2022 Behaviour merged from ToIntT()
				{
					double Double = 0;
					bool? IntPart = true;
					int FractionalPart = -1;
					bool isContainsDouble = false;
					bool negative = false;
					foreach (var ch in str)
					{
						if (IntPart == true)
						{
							if (CanBeNegative&&!isContainsDouble && ch == '-') negative = true; //:multiple ifs per cycle isn't affects perfomance; CanBeNegative не проверялось
							else if (!isContainsDouble && ch != ' ') negative = false;
							else if (ch.IsDigit())
							{
								if (Double > double.MaxValue / 10) return negative ? double.MinValue : double.MaxValue;
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Double = Double * 10 + (ch - '0');
								isContainsDouble = true;
							}
							else if (ch == Separator) IntPart = null; //: Состояние квантовой запутанности. Целая часть и закончилась или нет одновременно. Ну а на самом деле просто чтобы не парсил точку или запятую в предложении как разделитель
							else if(StopIfNumberEnded && isContainsDouble) break;
						}
						else
						{
							if (ch.IsDigit())
							{
								if(FractionalPart==15) break;
								IntPart = false;
								//:на случай строки вроде ".23" (=="0.23")
								if (!isContainsDouble && !CanBeShortcuted)
								{
									Double = Double * 10 + ch.ToIntT();
									isContainsDouble = true;
									IntPart = true;
									continue;
								}
								Double += ch.ToIntT()*Math.Pow(10, FractionalPart--);
							}
							else if (IntPart == null && DotShouldBeAttachedToNumber) IntPart = true;
							else if(StopIfNumberEnded && isContainsDouble) break;
						}
					}

					if (!isContainsDouble)
					{
						if (ExceptionValue == null)
						{
							throw new ArgumentException("Строка не содержит числа");
						}
						else return (int)ExceptionValue;
					}
					return negative? -Double:Double;
				}

				public static double? ToNDoubleT(this string str, char Separator = '.', bool CanBeNegative = true, bool CanBeShortcuted = true, bool DotShouldBeAttachedToNumber = true, bool StopIfNumberEnded = false) //:08.10.2021 new func
				{
					try
					{
						return ToDoubleT(str, CanBeNegative, StopIfNumberEnded, CanBeShortcuted, DotShouldBeAttachedToNumber, Separator);
					}
					catch (Exception)
					{
						return null;
					}
				}

		
				#endregion

				#region Float

				public static float FindFloat(this string str, char DecimalSeparator = '.', char thousandSeparator = ' ')
				{
					int start = 0;
					bool minus = false;
					while (!str[start].IsDigit())
					{
						minus = str[start] == '-';
						start++;
					}

					int end = minus ? start - 1 : start;
					do
					{
						end++;
					} while ((str[end].IsDigit() || str[end] == thousandSeparator || str[end] == DecimalSeparator) && end < str.Length - 1);

					return Convert.ToSingle(str.Substring(start, end - start).Replace(DecimalSeparator, '.').Replace(thousandSeparator.ToString(), ""), new CultureInfo("en-US", true));
				}


				#endregion

				#region Short

				public static short ReadShortUntilLetter(this string str)
				{
					short Short = 0;
					foreach (var ch in str)
					{
						if (ch.IsDigit())
						{
							Short = (short)(Short* 10 + (short)ch);
						}
						else break;
					}

					return Short;
				}

				#endregion

				#region Bool


				public static bool ToBool(this string S)
				{
					return S.ToLower() is "1" or "true" or "yes" or "да" or "是" or "si" or "sì" or "da" or "sim" or "ja" or "ya";
				}

				public static bool YesToBool(this string S)
				{
					return new[]
					{
						"1", "true",
						"是", // chinese (simplified)
						"yes", // english
						"sí", // spanish
						"sim", // portuguese
						"ya", // indonesian
						"हाँ", // hindi
						"نعم", // arabic
						"да", // russian
						"はい", // japanese
						"ja", // german
						"oui", // french
						"sì", // italian
						"evet", // turkish
						"ใช่", // thai
						"네", // korean
						"tak", // polish
						"ja", // dutch
						"igen", // hungarian
						"ja", // swedish
						"ano", // czech
						"vâng", // vietnamese
						"ναι", // greek
						"כן", // hebrew
						"da", // romanian
						"так", // ukrainian
						"да", // bulgarian
						"kyllä", // finnish
						"ja", // norwegian
						"ja", // danish
						"áno", // slovak
						"taip", // lithuanian
						"da", // croatian
						"да", // serbian
						"jah", // estonian
						"jā", // latvian
						"da", // slovenian
						"sí", // catalan
						"bəli", // azerbaijani
						"დიახ", // georgian
						"da", // serbian (latin)
						"так", // belarusian
						"да", // macedonian
						"иә", // kazakh
						"ஆம்", // tamil
						"bəli", // azerbaijani (cyrillic)
						"ਹਾਂ", // punjabi
					}.ContainsAny(S.ToLower());
				}

				public static string ToRuString(this bool Bool)
				{
					return Bool? "Да" : "Нет";
				}

				public static bool RandBool(int TrueProbability) {
					Random rand = new Random((int)DateTime.Now.Ticks);
					return rand.Next() <= TrueProbability;
				}

				#endregion

				#region Array

				public static double[,] ToDoubleT(this string[,] Strings, char Separator = '.')
				{
					int d0 = Strings.GetLength(0), d1 = Strings.GetLength(1);
					double[,] doubles = new double[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							doubles[i, j] = Strings[i, j].ToDoubleT(Separator:Separator);
						}
					}

					return doubles;
				}

				public static string[,] ToStringT(this double[,] Doubles, string Format)
				{
					int d0 = Doubles.GetLength(0), d1 = Doubles.GetLength(1);
					string[,] strings = new string[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							strings[i, j] = Doubles[i, j].ToString(Format);
						}
					}

					return strings;
				}

				public static string[,] ToStringMatrix(this double[,] Doubles)
				{
					int d0 = Doubles.GetLength(0), d1 = Doubles.GetLength(1);
					string[,] strings = new string[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							strings[i, j] = Doubles[i, j].ToString();
						}
					}

					return strings;
				}

				public static T[,,] ToSingleArray<T>(this T[][,] list)
				{
					int[] dimensions = new[] { list.Length, list[0].GetLength(0), list[0].GetLength(1) };

					T[,,] result = new T[dimensions[0],dimensions[1],dimensions[2]];

					try
					{

						for (int i = 0; i < dimensions[0]; i++)
					{
						for (int j = 0; j < dimensions[1]; j++)
						{
							for (int k = 0; k < dimensions[2]; k++)
							{
								result[i, j, k] = list[i][j, k];
							}
						}
					}

					}
					catch (ArgumentOutOfRangeException e)
					{
						throw new ArgumentException("iternal arrays' dimensions should be the same", e);
					}

					return result;
				}

				#endregion

			#endregion

			#endregion


		#region OtherTypeFuncs

			#region Int

			public static bool IsOdd(this int value) => value % 2 != 0;
			public static bool IsEven(this int value) => value % 2 == 0;

			public static int DivRem(int dividend, int divisor, out int reminder)
			{
				int quotient = dividend / divisor;
				reminder = dividend - divisor * quotient;
				return quotient;
			}

			
			/// <summary>
			/// Maxes input value be in range between MinValue and MaxValue
			/// </summary>
			public static int Fit(this int i, int MinValue = int.MinValue, int MaxValue = Int32.MaxValue) => i < MinValue ? MinValue : i > MaxValue ? MaxValue : i;

			/// <summary>
			/// Maxes input value be in range between 0 and MaxValue
			/// </summary>
			public static int FitPositive(this int i, int MaxValue = int.MaxValue) =>  i < 0 ? 0 : i > MaxValue ? MaxValue : i;
			/// <summary>
			/// Maxes input value be in range between MinValue and 0
			/// </summary>
			public static int FitNegative(this int i, int MinValue = int.MinValue) =>  i > 0 ? 0 : i < MinValue ? MinValue : i;


			#endregion

			#region String

			public static bool ContainsAny(this string s, IEnumerable<string> sequence) => sequence.Any(s.Contains); 
			public static bool ContainsAny(this string s, params string[] sequence) => sequence.Any(s.Contains); //:24.10.2022 IEnumerable replaced with params
			public static bool ContainsAll(this string s, IEnumerable<string> sequence) => sequence.All(s.Contains);//:24.10.2022 IEnumerable replaced with params

			public static bool ContainsAll(this string s, params string[] sequence) => sequence.All(s.Contains);//:24.10.2022 IEnumerable replaced with params

			public static int SymbolsCount(this string s)
			{
				int i = s.Length;
				foreach (var c in s)
				{
					if (char.IsControl(c)) i--;
				}

				return i;
			}

			#region SplitN

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <returns></returns>
				public static string[]? SplitN(this string s, string Separator)
				{
					var res = s.Split(Separator);
					return res[0]==s? null : res;
				}
				
				//:thanks GitHub Copilot for all other variations of this func. I hope he copied the summary right
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}
		
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, string Separator, int count)
				{
					var res = s.Split(Separator, count);
					return res[0]==s? null : res;
				}
				
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string Separator, int count, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, count, StringSplitOptions);
					return res[0]==s? null : res;
				}

				
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				public static string[]? SplitN(this string s, string[] Separator)
				{
					var res = s.Split(Separator, StringSplitOptions.None);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string[] Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, string[] Separator, int count)
				{
					var res = s.Split(Separator, count, StringSplitOptions.None);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string[] Separator, int count, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, count, StringSplitOptions);
					return res[0]==s? null : res;
				}


				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				public static string[]? SplitN(this string s, char Separator)
				{
					var res = s.Split(Separator);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, char Separator, int count)
				{
					var res = s.Split(Separator, count);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, char Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}

				#endregion

			public static string FormatToString(this double d, int n, Positon pos, char filler = ' ') //:Ленивый и неоптимизированный способ
			{
				d = Math.Round(d, n);
				string s = d.ToString();
				if (s.Length < n) {
					switch (pos) {
					case Positon.left: {
						for (int i = s.Length; i < n; i++)
						{
							s += filler;
						}
					} break;
					case Positon.center: {
						int halfString = (n - s.Length) / 2;
						for (int i = 0; i < halfString; i++)
						{
							s=s.Insert(0, filler.ToString());
						}
						for (int i = s.Length; i < n; i++)
						{
							s += filler;
						}
					}break;
					case Positon.right: {
						for (int i = 0; i < (n - s.Length); i++)
						{
							s = s.Insert(0, filler.ToString());
						}
					}break;
					}
				}
				else if (s.Length > n) {
					int Eindex = s.LastIndexOf('E');

					if (Eindex > 0) { //если в строке есть Е+хх
						string E = s.TrimStart('E');
						s = s.Substring(0, n - E.Length);
						s += E;
					}
					else {
						s = s.Substring(0, n);
					}
				}
				return s;
			}

			/// <summary>
			/// Makes s.Length be equal to <paramref name="FixedLength"/> by adding <paramref name="Offset"/> symbols if it's too short or cutting it if it's too long
			/// </summary>
			/// <param name="s"></param>
			/// <param name="FixedLengtharam">
			/// <param name="Align">The position of <see cref="s"/> if it's too short . If it's too long, it will be always aligned Left</param>
			/// <param name="Filter"></param>
			/// <param name="Offset">Positive is right, negative is left. Will be sliced if out of range</param>
			/// <returns></returns>
			public static string FormatString(this string s, int FixedLength, Positon Align, char Filter = ' ', int Offset = 0)
			{
				if (s.Length<FixedLength)
				{
					int NumberOfFillerSymbols = FixedLength - s.Length;
					switch (Align)
					{
						case Positon.left:
						{
							Offset = Offset.FitPositive(NumberOfFillerSymbols);
							NumberOfFillerSymbols -= Offset;
							s = Filter.Multiply(Offset) + s + Filter.Multiply(NumberOfFillerSymbols);
						}
							break;
						case Positon.center:
						{
							int firstHalf = NumberOfFillerSymbols.IsEven()? NumberOfFillerSymbols/2 : Offset>0? NumberOfFillerSymbols/2 : NumberOfFillerSymbols/2 +1,
								secondHalf = NumberOfFillerSymbols-firstHalf;
							if (NumberOfFillerSymbols.IsOdd()) Offset += (Offset > 0) ? -1 : 1;
									
							firstHalf += Offset;
							secondHalf -= Offset;
							s = Filter.Multiply(firstHalf) + s + Filter.Multiply(secondHalf);
						}
							break;
						case Positon.right:
						{
							Offset.FitNegative(NumberOfFillerSymbols);
							NumberOfFillerSymbols += Offset;
							s = Filter.Multiply(NumberOfFillerSymbols) + s + Filter.Multiply(Offset);
						}
							break;
					}
				}
				else if (s.Length>FixedLength)
				{
					int Eindex = s.LastIndexOf('E');

					if (Eindex>0)
					{ //если в строке есть Е+хх
						string E = s.TrimStart('E');
						s=s.Substring(0, FixedLength-E.Length);
						s+=E;
					}
					else
					{
						s=s.Substring(0, FixedLength);
					}
				}
				return s;
			}

			/// <summary>
			/// Inserts <paramref name="Value"/> between <paramref name="Start"> and <paramref name="end"/>
			/// </summary>
			/// <param name="s"></param>
			/// <param name="Value"></param>
			/// <param name="Start"></param>
			/// <param name="end"></param>
			/// <returns></returns>
			/// <exception cref="ArgumentException"></exception>
			public static string Insert(this string s, string Value, int Start, int? End = null)
			{
				int end=  End?? Start;
				if (s.IsNullOrEmpty()) return Value;
				if (Start < 0) Start = s.Length - Start;
				if (end < 0) end = s.Length - Start;
				if (Start > end) Swap(ref Start, ref end);

				if (Start < 0 || Start > s.Length) throw new ArgumentException("Incorrect start " + Start, nameof(Start));
				if (end < 0 || end > s.Length) throw new ArgumentException("Incorrect end " +end, nameof(end));

				return s.Slice(0,Start) + Value + s.Slice(Start);
			}

			public enum Positon: byte { left,center,right}

		/// <summary>
		/// Slices the string form <paramref name="Start"/> to <paramref name="End"/> <para></para>
		/// Supported types: <typeparamref name="int"></typeparamref>, <typeparamref name="string"></typeparamref>, <typeparamref name="Regex"></typeparamref>, <typeparamref name="Func&lt;char,bool&gt;"/>.
		/// </summary>
		/// <typeparam name="Ts">Type of the <paramref name="Start"/></typeparam>
		/// <typeparam name="Te">Type of the <paramref name="End"/></typeparam>
		/// <param name="s"></param>
		/// <param name="Start"> Start of the result string <para/>
		///<list type="table"></list>
		/// /// <item><typeparamref name="default"/>: 0 (don't cut start)</item>
		/// <item><typeparamref name="int"/>: Start index of the result string (inverse direction if negative)</item>
		/// <item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the start position of the result</item>
		/// <item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the start position of the result</item>
		/// <item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</item>
		/// </param>
		/// <param name="End">End of the result string <para></para>
		///<list type="table"></list>
		///  <item><typeparamref name="default"/>: Max (don't cut end)</item>
		/// <item><typeparamref name="int"/>: End index of the result string (inverse direction if negative)</item>
		/// <item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the end position of the result</item>
		/// <item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the end position of the result</item>
		/// <item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы конца строки (по функции на 1 символ)</item>
		/// </param>
		/// <param name="AlwaysReturnString">return <paramref name="s"/> if <paramref name="Start"/> or <paramref name="End"/> not found (may be half-cutted)</param>
		/// <param name="LastStart">if true, the last occurance of the <paramref name="Start"/> will be searched <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
		/// <param name="LastEnd">if true, the last occurance of the <paramref name="End"/> will be searched <para/> (doesn't do anything if <paramref name="End"/> is <typeparamref name="int"/>)</param>
		/// <param name="IncludeStart">Include <paramref name="Start"/> symbols <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
		/// <param name="IncludeEnd">Include <paramref name="End"/> symbols <para/> (doesn't do anything if <paramref name="End"/> is <typeparamref name="int"/>)</param>
		/// <returns></returns>
		/// <exception cref=""></exception>
		public static string Slice<Ts, Te>(this string s, Ts? Start, Te? End, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false)
		{
			if (s.IsNullOrEmpty())
				if (AlwaysReturnString)
					return null;
				else
					throw new ArgumentNullException(nameof(s));

			int start;
			int end;
			bool BasicSlice = Start is int or null && End is int or null;

			switch (Start)
			{
				case null:
						start = 0;
					break;
				case int startIndex:
					start = startIndex;
					if (start < 0) start = s.Length + start; //: count from end if negative
					if (start < 0 || start >= s.Length)
						if (AlwaysReturnString)
							start = 0;
						else
							return null;
					break;
				case string startsWith:
					start = LastStart ? s.LastIndexOfEnd(startsWith) : s.IndexOfEnd(startsWith);
					if (start < 0) start = 0;
					if (IncludeStart) start += startsWith.Length;
					break;
				case Regex startRegex:
					var match = LastStart? startRegex.Matches(s).Last() :  startRegex.Match(s);
					start = match.Index>=0? 
						(match.Index + (IncludeStart ? 0 : match.Length)) : 0;
					break;
				case Func<char,bool>[] startConditions:
					start = startConditions?.Any()==true? 
						s.IndexOfT(startConditions, IndexOfEnd: !IncludeStart, RightDirection: !LastStart) : 0;
					if (start < 0) start = 0;
					break;
				default:
					throw new TypeInitializationException(typeof(Ts).FullName, new ArgumentException($"Type of {nameof(Start)} is not supported"));
			}

			if (!BasicSlice) s = s.Slice(start);

			switch (End)
			{
				case null:
						end = s.Length;
					break;
				case int endIndex:
					end = endIndex;
					if (end < 0) end = s.Length + end; //: count from end if negative
					if (BasicSlice && start > end) Swap(ref start, ref end);
					if (end > s.Length) end = s.Length;
					break;
				case string endsWith:
					end = (LastEnd ? s.LastIndexOf(endsWith) : s.IndexOf(endsWith));
					if(end<0) end = s.Length;
					if (IncludeEnd) end += endsWith.Length;
					break;
				case Regex endregex:
					var match = LastEnd? endregex.Matches(s).Last() :  endregex.Match(s);
					end = match.Index>=0? 
						(match.Index + (LastEnd ? 0 : match.Length)) : 0;
					break;
				case Func<char,bool>[] endConditions:
					end = endConditions?.Any()!=true? 
						s.IndexOfT(endConditions,IndexOfEnd: IncludeEnd, RightDirection: !LastEnd) : 0;
					if (end < 0) 
						if (AlwaysReturnString)
							end = s.Length-1;
						else
							return null;
					break;
				default:
					throw new TypeInitializationException(typeof(Ts).FullName, new ArgumentException($"Type of {nameof(End)} is not supported"));
			}

			return BasicSlice ?
				s.Substring(start, (end - start)) :
				s.Slice(0, end);
		}

		/// <summary>
		/// Removes <paramref name="s"/> symbols from 0 to <paramref name="Start"/><para></para>
		/// Supported types: <typeparamref name="int"></typeparamref>, <typeparamref name="string"></typeparamref>, <typeparamref name="Regex"></typeparamref>, <typeparamref name="Func&lt;char,bool&gt;"></typeparamref>;
		/// </summary>
		/// <typeparam name="Ts">Type of the <paramref name="Start"/></typeparam>
		/// <param name="s"></param>
		/// <param name="Start"> Start of the result string <para/>
		///<list type="table"></list>
		/// /// <item><typeparamref name="default"/>: 0 (don't cut start)</item>
		/// <item><typeparamref name="int"/>: Start index of the result string (inverse direction if negative)</item>
		/// <item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the start position of the result</item>
		/// <item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the start position of the result</item>
		/// <item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</item>
		/// </param>
		
		/// <param name="AlwaysReturnString">return <paramref name="s"/> if <paramref name="Start"/> or <paramref name="End"/> not found (may be half-cutted)</param>
		/// <param name="LastStart">if true, the last occurance of the <paramref name="Start"/> will be searched <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
		/// <param name="IncludeStart">Include <paramref name="Start"/> symbols <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
		
		/// <returns></returns>
		/// <exception cref=""></exception>
		public static string Slice<Ts>(this string s, Ts? Start, bool AlwaysReturnString = false, bool LastStart = false, bool IncludeStart = false) =>
			s.Slice(Start, int.MaxValue, AlwaysReturnString, LastStart, false, IncludeStart, false);

		public static string SliceFromEnd(this string s, string StartsWith = null, string EndsWith = null, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false) //:25.08.2022 includeStart, includeEnd
			{
				var end = EndsWith==null? s.Length-1 : LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
				if (end < 0) return  AlwaysReturnString? s : null;

				s = s.Slice(0, end);

				var start = StartsWith == null? 0 : LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
				if (start < 0) return  AlwaysReturnString? s : null;

				return IncludeStart? StartsWith : "" + s.Slice(start) + (IncludeEnd? EndsWith : "");
			}

			private static int IndexOfEnd(this string s, string s2)
			{
				if (s == null)
					if (s2.Length == 0)
						return 0;
				int i = s.IndexOf(s2);
				return i == -1 ? -1 : i + s2.Length;
			}

			/// <summary>
			/// Reports the position of a symbol next to last occurance of a string <paramref name="s2"/> (position after the end of <paramref name="s2"/>) 
			/// </summary>
			/// <param name="s"></param>
			/// <param name="s2"></param>
			/// <returns></returns>  
			private static int LastIndexOfEnd(this string s, string s2)
			{
				if (s == null) 
					if (s2.Length == 0) return 0;
				int i = s.LastIndexOf(s2);
				if (i == -1) return -1;
				else return i + s2.Length;
			}

			public enum DirectionEnum
			{
				Custom,
				Right,
				Left
			}

			public static int IndexOfT(this string s, Func<char,bool>[] Conditions, int Start = 0, int End = Int32.MaxValue, bool RightDirection = true, bool IndexOfEnd = false) //:22.09.22 Bugfix, deleted useless LastOccurance; Replaced DirectionEnum with bool RightDirection
			{
				if (End == Int32.MaxValue) End = s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");

				if (End == Start) return -2;

				if(RightDirection && End < Start ||
				   !RightDirection && End > Start)
					Swap(ref Start, ref End);
				
				int defaultCurMatchPos = RightDirection? 0 : Conditions.Length-1;
				int curCondition = defaultCurMatchPos;
				int Result = -1;

				if (RightDirection)
					for (int i = Start; i < End; i++)
					{
						if (Conditions[curCondition](s[i]))
						{
							curCondition++;
							if (curCondition != Conditions.Length) continue;
							Result = i;
							curCondition = defaultCurMatchPos;
							//if(!LastOccuarance)
								break;
						}
						else
						{
							i -= curCondition;
							curCondition = defaultCurMatchPos;
						}
					}
				else
					for (int i = Start; i >= End; i--)
					{
						if (Conditions[curCondition](s[i]))
						{
							curCondition--;
							if (curCondition != 0) continue;
							Result = i;
							curCondition = defaultCurMatchPos;
							//if(!LastOccuarance) 
								break;
						}
						else
						{
							i += ((Conditions.Length-1) - curCondition);
							curCondition = defaultCurMatchPos;
						}
					}

				return Result = Result == -1 || IndexOfEnd ^ !RightDirection?
					Result : (Result - Conditions.Length) +1;
			}

			/// <summary>
			/// Return an index of (<paramref name="IndexOfEnd"/>? end : start) of <paramref name="s2"/> in <paramref name="s"/>
			/// </summary>
			/// <param name="s"></param>
			/// <param name="s2"></param>
			/// <param name="Start">Start checking from</param>
			/// <param name="End">until End</param>
			/// <param name="Direction">Direction of searching</param>
			/// <param name="IndexOfEnd">Return index of end of s2 (last letter) instead of start (first letter)</param>
			/// <param name="LastOccuarance"></param>
			/// <returns></returns>
			/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static int IndexOfT(this string s, string s2, int Start = 0, int End = Int32.MaxValue, bool RightDirection = true, bool IndexOfEnd = false) //:22.09.22 Bugfix, deleted useless LastOccurance; Replaced DirectionEnum with bool RightDirection
			{
				if (End == Int32.MaxValue) End = s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");

				if (End == Start) return -2;

				if(RightDirection && End < Start ||
				   !RightDirection && End > Start)
					Swap(ref Start, ref End);

				int defaultCurMatchPos = RightDirection? 0 : s2.Length-1;
				int curMatchPos = defaultCurMatchPos;
				int Result = -1;

				if (RightDirection)
					for (int i = Start; i < End; i++)
					{
						if (s[i] == s2[curMatchPos])
						{
							curMatchPos++;
							if (curMatchPos != s2.Length) continue;
							Result = i;
							curMatchPos = defaultCurMatchPos;
							//if(!LastOccuarance)
								break;
						}
						else
						{
							i -= curMatchPos;
							curMatchPos = defaultCurMatchPos;
						}
					}
				else
					for (int i = Start; i >= End; i--)
					{
						if (s[i] == s2[curMatchPos])
						{
							curMatchPos--;
							if (curMatchPos != 0) continue;
							Result = i;
							curMatchPos = defaultCurMatchPos;
							//if(!LastOccuarance) 
								break;
						}
						else
						{
							i += ((s2.Length-1) - curMatchPos);
							curMatchPos = defaultCurMatchPos;
						}
					}

				return Result = Result == -1 || IndexOfEnd ^ !RightDirection?
					Result : (Result - s2.Length) +1;
			}


			public static string Multiply(this string s, int count)
			{
				StringBuilder sb = new StringBuilder(s.Length*count);
				for (int i = 0; i < count; i++)
				{
					sb.Append(s);
				}

				return sb.ToString();
			}

			public static string Multiply(this char s, int count)
			{
				StringBuilder sb = new StringBuilder(count);
				for (int i = 0; i < count; i++)
				{
					sb.Append(s);
				}

				return sb.ToString();
			}


			/// <summary>
			/// <paramref name="s"/>[..<paramref name="Start"/>] + <paramref name="NewString"/> + s[<paramref name="End"/>..]
			/// </summary>
			/// <param name="s">original string</param>
			/// <param name="Start">String from 0 to Start will be added before NewString</param>
			/// <param name="End">String from End to s.Length-1 will be added after NewString. If null, End = NewString.Length, or null if there's no NewString</param>
			/// <param name="NewString">String that will be between Start and End. If null, you'll just cut the text between Start and End</param>
			/// <param name="Exception">if false, its returns original string instead of exception</param>
			/// <returns></returns>
			public static string Replace(this string s, int Start, int? End = null, string NewString = null, bool Exception = true)
			{
				if (Start < 0) Start = s.Length - Start;
				if (Start < 0 || Start > s.Length) 
					if (Exception) throw new ArgumentOutOfRangeException($"There's no position {Start} in string s[{s.Length}]"); 
					else return s;
				
				if (End != null)
				{
					if (End < 0) End = s.Length - End;
					if (End < 0 || End > s.Length)
						if (Exception) throw new ArgumentOutOfRangeException($"There's no position {End} in string s[{s.Length}]");
						else if (End > s.Length) End = s.Length - 1;
							else return s;
				}
				else End = Start + NewString?.Length?? 0;

				if (Start == End && string.IsNullOrEmpty(NewString)) return s;
				if (End < s.Length) return s[..Start] + NewString + s[(int)End..];
				else return s[..Start] + NewString;
			}


			public static string ReplaceFromLast(this string s, string OldString, string NewString, bool Exception = true)
			{
				if (OldString is null)
				{
					if (Exception) throw new ArgumentNullException(nameof(OldString));
					else return s;
				}
				var start = s.LastIndexOf(OldString, StringComparison.Ordinal);
				return s.Replace(start, start+OldString.Length, NewString, Exception);
			}

			public static string Replace(this string s, Dictionary<string, string> ReplaceRule) => s.Replace(ReplaceRule.Keys, ReplaceRule.Values);

			public static string Replace(this string s, IEnumerable<string> OldStrings, IEnumerable<string> NewStrings)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Escapes string
			/// </summary>
			/// <param name="s"></param>
			/// <param name="Characters">characters need to be escaped</param>
			/// <returns></returns>
			public static string Escape(this string s, string Characters, string EscapeSymbol = "\\") //:26.08.2022
			{
				string result = "";
				foreach (var c in s)
				{
					if (Characters.Contains(c)) result += EscapeSymbol;
					result += c;
				}

				return result;
			}

			public static IEnumerable<string> Add(this IEnumerable<string> strings, string addiction, bool ToEnd = true)
			{
				var list = strings.ToList();
				for (var index = 0; index < list.Count; index++)
				{
					list[index] = list[index].Add(addiction, ToEnd);
				}

				return list;
			}

			/// <summary>
			/// Добавляет ту часть <paramref name="addiction"/> к <paramref name="s"/>, которая не содержится в конце <paramref name="s"/>. Например, "Test".Add("stop") = "Testop"; "Test".Add("rest") = "Testrest"
			/// </summary>
			/// <param name="s"></param>
			/// <param name="addiction"></param>
			/// <param name="ToEnd">Добавить к концу строки (иначе -- к началу)</param>
			/// <returns></returns>
			public static string Add(this string s, string addiction, bool ToEnd = true) => ToEnd ? AddToEnd(s, addiction) : AddToStart(s, addiction); //TODO: ToEnd лучше заменить с bool на enum Position{Start,End}

			private static string AddToStart(this string s, string addiction)
			{
				if (addiction.Length>1) throw new NotImplementedException("AddToEnd with more than 1 character isn't supported yet");

				if (s[0] != addiction[0]) return addiction + s;
				else return s;
			}

			private static string AddToEnd(this string s, string addiction)
			{
				if (s.IsNullOrEmpty()) return addiction;
				int offset = 0;

				for (; offset < addiction.Length; offset++)
				{
					if (s[^1] != addiction[offset]) continue;

					int aPosition = offset;
					for (int sPosition = s.Length-1; sPosition >=0;)
					{

						if (s[sPosition--] != addiction[aPosition--])
						{
							Debug.Print($"{s[..(sPosition+1)]}|{s[sPosition+1]}|{s[(sPosition+2)..]} ≠ {addiction[..(aPosition+1)]}|{addiction[aPosition+1]}|{addiction[(aPosition+2)..]}");
							break;
						}
						if (aPosition < 0)
						{
							return s + addiction[(offset+1)..];
						}
					}
				}

				return s+addiction;
			}


			#endregion

			#region Double

			public static bool isEven(this int number) => number % 2 == 0;

			#endregion

			#region Enumerable

			public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] values) => source.Any(values.Contains);
			public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] values) => source.Any(values.Contains);
			public static bool Empty<T>(this IEnumerable<T> s) => !s.Any(); 
			public static Tgt[] ToArray<Tgt, Src>(this IEnumerable<Src> source, Func<Src, Tgt> Convert)
			{
				Tgt[] result = new Tgt[source.Count()];
				int i = 0;
				foreach (var s in source)
				{
					result[i++] = Convert(s);
				}

				return result;
			}

			/// <summary>
			/// Случайным образом перемешивает массив
			/// </summary>
			public static List<T> RandomShuffle<T>(this IEnumerable<T> list)
			{
				Random random = new Random();
				var shuffle = new List<T>(list);
				for (var i = shuffle.Count() - 1; i >= 1; i--)
				{
					int j = random.Next(i + 1);

					var tmp = shuffle[j];
					shuffle[j] = shuffle[i];
					shuffle[i] = tmp;
				}
				return shuffle;
			}

			public static List<T> RandomShuffle<T>(this IEnumerable<T> list, Random random)
			{
				var shuffle = new List<T>(list);
				for (var i = shuffle.Count() - 1; i >= 1; i--)
				{
					int j = random.Next(i + 1);

					var tmp = shuffle[j];
					shuffle[j] = shuffle[i];
					shuffle[i] = tmp;
				}
				return shuffle;
			}

			public static List<int> RandomList(int start, int count)
			{
				List<int> List = new List<int>(count);
				List<bool> Empty = new List<bool>();
				for (int i = 0; i < count; i++)
				{
					List.Add(0);
					Empty.Add(true);
				}
				Random Random = new Random();

				int End = start + count;
				for (int i = start; i < End;)
				{
					int Index = Random.Next(0, count); //C#-повский рандом гавно. Надо заменить чем-то

					if (Empty[Index])
					{
						List[Index] = i;
						Empty[Index] = false;
						i++;

					}
				}

				return List;
			}

			public static T Pop<T>(this List<T> list)
			{
				T r = list[^1];
				list.RemoveAt(list.Count-1);
				return r;
			}
		
			public static void Swap<T>(this List<T> list, int aIndex, int bIndex)
			{
				T value = list[aIndex];
				list[aIndex] = list[bIndex];
				list[bIndex] = value;
			}

			public static void Swap<T>(this T[] list, int aIndex, int bIndex)
			{
				T value = list[aIndex];
				list[aIndex] = list[bIndex];
				list[bIndex] = value;
			}
			public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);

			public static T[][] Split<T>(this T[] array, int arraysCount)
			{
				var arraysSize = DivRem(array.Length,arraysCount,out int lastArraySize);
				if (lastArraySize == 0) lastArraySize = arraysSize;

				T[][] resultArrays = new T[arraysCount][];
				int k = 0;
				for (int i = 0; i < arraysCount-1; i++)
				{
					resultArrays[i] = new T[arraysSize];
					for (int j = 0; j < arraysSize; j++)
					{
						resultArrays[i][j] = array[k++];
					}
				}

				resultArrays[^1] = new T[lastArraySize];
				for (int j = 0; j < lastArraySize; j++)
				{
					resultArrays[^1][j] = array[k++];
				}

				return resultArrays;
			}
			public static int GetMaxIndex(this double[] array)
			{
				int MaxIndex = 0;
				double MaxValue = array[0];

				for (int i = 1; i < array.Length; i++)
				{
					if (array[i] > MaxValue)
					{
						MaxValue = array[i];
						MaxIndex = i;
					}
				}

				return MaxIndex;
			}
			public static T[] Concat<T>(T[] Array1, T[] Array2)
			{
				T[] res = new T[Array1.Length + Array2.Length];
				for (int i = 0; i < Array1.Length; i++)
				{
					res[i] = Array1[i];
				}

				for (int i = 0; i < Array2.Length; i++)
				{
					res[i + Array1.Length] = Array2[i];
				}

				return res;
			}

			public static T[] ReduceDimension<T>(this T[][] Arrays)
			{
				int arraySize = 0;
				for (int i = 0; i < Arrays.Length; i++)
				{
					arraySize += Arrays[i].Length;
				}

				T[] res = new T[arraySize];
				int k = 0;
				foreach (var array in Arrays)
				{
					foreach (var item in array)
					{
						res[k++] = item;
					}
				}

				return res;
			}

			public static string ToStringLine<T>(this IEnumerable<T> Array, string Separator = " ")
			{
				string result = "";
				foreach (var el in Array)
				{
					result += el + Separator;
				}

				return result[..^Separator.Length];
			}

			public static string ToStringLine<T>(this IEnumerable<T> Array, string Separator, string LastSeparator)
			{
				string result = "";
				foreach (var el in Array)
				{
					result += el + Separator;
				}

				return result[..^Separator.Length];
			}

			public static T[] FillAndGet<T>(this T[] source, T value)
			{
				for (int i = 0; i < source.Length; i++)
					source[i] = value;

				return source;
			}

			public static T[] AddRangeAndGet<T>(this T[] array, T[] summand)
			{
				var newArray = new T[array.Length + summand.Length];
				for (int i = 0; i < array.Length; i++)
				{
					newArray[i] = array[i];
				}
				for (int i = 0; i < summand.Length; i++)
				{
					newArray[i + array.Length] = summand[i];
				}

				return newArray;
			}

			public static List<T> AddRangeAndGet<T>(this List<T> list, List<T> summand)
			{
				list.AddRange(summand);
				return list;
			}

			/// <summary>
			/// Removes (<paramref name="FromLeft"/>? first : last) occurance of <paramref name="RemovableString"/>
			/// </summary>
			/// <param name="S"></param>
			/// <param name="RemovableString"></param>
			/// <param name="FromLeft"></param>
			/// <param name="ComparisonType"></param>
			/// <returns></returns>
			public static string Remove(this string S, string RemovableString, bool FromLeft = true, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				if (RemovableString is null or "") return S;
				int startPos = FromLeft? S.IndexOf(RemovableString) : S.LastIndexOf(RemovableString);
				return startPos == -1 ? S : S.Remove(startPos, RemovableString.Length);
			}

			public static string RemoveFrom(this string Source, TypesFuncs.Side FromWhere = Side.End, params string[] RemovableStrings)
			{
				foreach (var rem in RemovableStrings) 
					Source = Source.RemoveFrom(FromWhere, rem);
				return Source;
			}

			public static string RemoveFrom(this string Source, Side FromWhere, string RemovableString)
			{
				if (FromWhere!= Side.End && Source.StartsWith(RemovableString)) Source = Source.Slice(RemovableString.Length);
				if (FromWhere!= Side.Start && Source.EndsWith(RemovableString)) Source = Source.Slice(0, -RemovableString.Length);
				return Source;
			}

			public static string RemoveAll(this string S, string RemovableString, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				if (RemovableString is null or "") return S;

				while (true)
				{
					int startPos = S.IndexOf(RemovableString,ComparisonType);
					if (startPos == -1) return S;

					S = S.Remove(startPos, RemovableString.Length);
				} 
			}

			public static string RemoveAll(this string S, string[] RemovableStrings, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				foreach (var s in RemovableStrings)
				{
					S = S.RemoveAll(s, ComparisonType);
				}

				return S;
			}

			public enum Side
			{
				Start,
				End,
				Both
			}

			public static string RemoveAllFrom(this string S, string RemovableChars, Side FromWhere = Side.Both, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				int start = 0, end = 0;

				if (FromWhere != Side.End)
					foreach (var C in S)
					{
						if (RemovableChars.Contains(C)) start++;
						else break;
					}

				if (FromWhere != Side.Start)
					for (int i = S.Length -1; i >=0; i--)
					{
						if (RemovableChars.Contains(S[i])) end++;
						else break;
					}

				return S[start..^end];
			}

			public static bool AllEquals<T>(this IEnumerable<T> array) => array.All(x => Equals(array.First(), x));

			#endregion

			#region Size

				// returns the highest dimension
				public static int Max(this Size s) => Math.Max(s.Height, s.Width);
				// returns the lowest dimension 
				public static int Min(this Size s) => Math.Min(s.Height, s.Width);

				/// <summary>
				/// Returns the new Size with same aspect ratio
				/// </summary>
				/// <param name="s">original size</param>
				/// <param name="NewDimensionValue"></param>
				/// <param name="FixedDimension">Dimension you just wrote</param>
				/// <returns></returns>
				public static Size Resize(this Size s, int NewDimensionValue, Dimension FixedDimension = Dimension.Height)
				{
					if ((FixedDimension is Dimension.Height && s.Height == 0) || (FixedDimension is Dimension.Width && s.Width == 0)) throw new ArgumentOutOfRangeException(nameof(s), $"original {FixedDimension} can't be 0");

					return FixedDimension switch
					{
						Dimension.Height => new Size((s.Width * NewDimensionValue) / s.Height, NewDimensionValue),
						Dimension.Width => new Size(NewDimensionValue, (s.Height * NewDimensionValue) / s.Width),
						_ => throw new ArgumentOutOfRangeException(nameof(FixedDimension), FixedDimension, null)
					};
				}

				public enum Dimension
				{
					Height,
					Width
				}
			#endregion

			#region Regex

			public static bool IsMatchT(this Regex r, string s, int start = 0) => s != null && r.IsMatch(s, start);
			public static bool IsMatchAny(this IEnumerable<Regex> r, string s, int start = 0) => s != null && r.Any(x => x.IsMatch(s));
			public static bool IsMatchAll(this IEnumerable<Regex> r, string s, int start = 0) => s != null && r.All(x => x.IsMatch(s));
			public static Regex Reverse(this Regex r)
			{
				var regStr = r.ToString();
				return regStr.StartsWith(@"^((?!") && regStr.EndsWith(@").)*")? new Regex(regStr[5..^4]) : new Regex(@"^((?!" + r + @").)*");
			}

			public static List<Regex> ReverseRegexes(this List<Regex> r)
			{
				for (int i = 0; i < r.Count; i++)
				{
					r[i] = Reverse(r[i]);
				}

				return r;
			}

			/// <summary>
			/// Swaps "equal" mode to "contains" mode (Добавляет $/^ в начало/конец, если их нет; Убирает их, если они есть)
			/// </summary>
			/// <param name="S"></param>
			/// <returns></returns>
			internal static Regex InvertRegex(string S)
			{
				bool anyStart = S.StartsWith("^");
				bool anyEnd = S.EndsWith("$");
				S = anyStart ? S[1..] : $"^{S}";
				S = anyEnd ? S[..^1] : $"{S}$";
				return new Regex(S);
			}

			#endregion

			#region Dictionary

			/*public static Dictionary Reverse<Tkey, TValue>(this Dictionary<Tkey, TValue> source)
			{
				for (int i = 0; i < source.Count; i++)
				{
					source.
				}
			}*/

			#endregion

			#region Color

			static Color Change(this Color c, byte? A = null, byte? R = null, byte? G = null, byte? B = null) => Color.FromArgb(A?? c.A, R??c.R, G??c.G, B??c.B);
			
			#region Universal Type

			public static void Swap<T>(ref T a, ref T b)
			{
				T c = a;
				a = b;
				b = c;
			}

			public static bool IsDefault<T>(this T value) where T : struct
			{
				bool isDefault = value.Equals(default(T));

				return isDefault;
			}													 

			public static IEnumerable<T> ToDefault_IfNot<T>(this IEnumerable<T>  s, Func<T, bool> predicate)  => s?.Any(predicate) is true ? s : null; 
			public static T ToDefault_IfNot<T>(this T s, Func<T, bool> predicate) where T : struct => predicate(s)? s : default;

			#endregion

			#endregion

			#region Process

			public static void KillProcesses(string? Path = null, string? Name = null)
			{
				List<Process> processes;
				Name ??= Path?.Slice(new[] { new Func<char, bool>(x => x is '\\' or '/') }, LastStart:true);
				try
				{
					processes = (
						from proc in Name == null ? Process.GetProcesses() : Process.GetProcessesByName(Name)
						where Path == null || proc.MainModule.FileName == Path
						select proc
					).ToList();
				}
				catch (Exception e)
				{
					throw new Exception("Error while gathering processes", e);
				}

				var Exceptions = new List<InvalidOperationException>();

				foreach (var proc in processes)
				{
					try
					{
						proc.Kill();
					}
					catch (Exception e)
					{
						Exceptions.Add(new InvalidOperationException($"Can't kill process {proc.ProcessName}", e));
					}
				}

				if (Exceptions.Count > 0) throw new AggregateException(Exceptions);
			}

			#endregion

		#endregion
	}

	public static class IO
	{
		/// <summary>
		/// Copies all files, directories, subdirectories and it's contant to the new folder
		/// </summary>
		/// <param name="SourcePath"></param>
		/// <param name="TargetPath"></param>
		/// <param name="KillRelatedProcesses"></param>
		/// <param name="DisableSyntaxCheck">All paths should end on "\" and contains only "\" (not "/)</param>
		public static void CopyAll(string SourcePath, string TargetPath, bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false)
		{
			ExceptList ??= new List<Regex>();
			var ErrorList = new List<Exception>();
			if (!DisableSyntaxCheck)
			{
				SourcePath = SourcePath.Replace("/", "\\").Add("\\");
				TargetPath = TargetPath.IsNullOrEmpty()? "" : TargetPath.Replace("/", "\\").Add("\\");
			}

			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
			{
				if (ExceptList.IsMatchAny(dirPath)) continue;
				try
				{
					Directory.CreateDirectory(dirPath.Replace(SourcePath, TargetPath));
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
				
			}

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
			{
				if (ExceptList.IsMatchAny(newPath)) continue;
				try
				{
					var destination = newPath.Replace(SourcePath, TargetPath);
					if (KillRelatedProcesses && destination.EndsWith(".exe"))
						TypesFuncs.KillProcesses(Path: destination);
					File.Copy(newPath, destination , true);
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
				
			}

			if (ErrorList.Count > 0) throw new AggregateException("Unable to copy files" ,ErrorList);
		}

		public static void RemoveAll(string FolderPath, bool RemoveSelf = true, List<Regex>? ExceptList = null)
		{
			var ErrorList = new List<Exception>();

			foreach (var dir in  Directory.GetDirectories(FolderPath))
			{
				try
				{
					if(ExceptList?.IsMatchAny(dir)?? false) continue;
					RemoveAll(dir, ExceptList:ExceptList);
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
			}

			foreach (var file in Directory.GetFiles(FolderPath))
			{
				if(ExceptList?.IsMatchAny(file)?? false) continue;
				File.Delete(file);
			}

			if (RemoveSelf) 
				try { Directory.Delete(FolderPath, false);}
				catch (Exception e) {ErrorList.Add(e);}

			if (ErrorList.Count > 0) throw new AggregateException("Unable to copy files" ,ErrorList);
		}

		/// <summary>
		///  Renames all files in the Directory (not recursive)
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <param name="Rename"> Function where input is file's name (not path) and the output is new file's name</param>
		/// <param name="ExceptList">Regular expression of filePATHs that won't be renamed</param>
		public static void RenameAll(string FolderPath, Func<string, string> Rename, List<Regex>? ExceptList = null)
		{
			ExceptList ??= new List<Regex>();
			foreach (var file in Directory.GetFiles(FolderPath))
			{
				if(ExceptList.IsMatchAny(file)) continue;

				var fileInfo = new FileInfo(file);
				var path = fileInfo.Directory.FullName;
				var fileName = fileInfo.Name;
				File.Move(file,Path.Combine(path, Rename(fileName)));	
			}
		}

		public static void CopyAllTo(this DirectoryInfo di, string TargetPath, bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false)
		{
			CopyAll(di.FullName, TargetPath, KillRelatedProcesses, ExceptList, DisableSyntaxCheck);
		}

		public static void MoveAllTo(string SourcePath, string TargetPath, bool DeleteSourceDir = true,  bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false) => new DirectoryInfo(SourcePath).MoveAllTo(TargetPath, DeleteSourceDir, KillRelatedProcesses, ExceptList, DisableSyntaxCheck);

		public static void MoveAllTo(this DirectoryInfo di, string TargetPath, bool DeleteSourceDir = true, bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false)
		{
			var sourcePath = di.FullName;
			CopyAll(sourcePath,TargetPath,KillRelatedProcesses, ExceptList, DisableSyntaxCheck);
			if (DeleteSourceDir) Directory.Delete(sourcePath, true);
			else
			{
				foreach (var dir in di.GetDirectories())
				{
					if (ExceptList.IsMatchAny(dir.FullName))
					dir.Delete(true);
				}
			}
		}
	}

	public static class ClassicFuncs
	{
		public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

		public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);
		/// <summary>
		/// Retrieves reference to specified string
		/// </summary>
		/// <param name="s"></param>
		/// <returns>A reference to s if it is in the common language runtime intern pool; otherwise null</returns>
		public static string IsInterned(this string s) => string.IsInterned(s);

	}

	public static class Classes
	{
		public class FileSize
		{
			private long longSize;

			public double Size;
			public SizeUnit Unit;

			public enum SizeUnit
			{
				Bit,
				Byte,
				KiloByte,
				MegaByte,
				GigaByte,
				TeraByte,
				PetaByte,
				ExaByte,
				ZettaByte,
				YottaByte,
				KiloBit,
				MegaBit,
				GigaBit,
				TeraBit,
				PetaBit,
				ExaBit,
				ZettaBit,
				YottaBit
			}

			private static string[][] UnitNames = new[]
			{
				new[] { "bit" },
				new[] { "byte", "b" },
				new[] { "kilobyte", "kb" },
				new[] { "megabyte", "mb" },
				new[] { "gigabyte", "gb" },
				new[] { "terabyte", "tb" },
				new[] { "petabyte", "pb" },
				new[] { "exabyte", "eb" },
				new[] { "zettabyte", "zb" },
				new[] { "yottabyte", "yb" },
				new[] { "kilobit", "kbit" },
				new[] { "megabit", "mbit" },
				new[] { "gigabit", "gbit" },
				new[] { "terabit", "tbit" },
				new[] { "petabit", "pbit" },
				new[] { "exabit", "ebit" },
				new[] { "zettabit", "zbit" },
				new[] { "yottabit", "ybit" }
			};

			private void CalculateLongSize()
			{
				int intUnit = (int)Unit;
				longSize = (long)(
					(SizeUnit)(Size * (double)Unit) ==
					SizeUnit.Bit ? 1 :
					Unit == SizeUnit.Byte ? 8 :
					Math.Pow(2, (intUnit - 2) % 8 + 1) * (Unit > SizeUnit.YottaByte ? 1 : 8)
				);
			}

			private static string getUnitName(SizeUnit SU) => UnitNames[(int)SU][0];

			private static SizeUnit getSizeUnit(string UnitName, bool strictlyEqual = false)
			{
				var lowerUnitName = UnitName.ToLower();

				for (int i = UnitNames.Length-1; i >=0; i--)
				{
					//Debug.Write(UnitNames[i].ToStringT(", "));
					if (strictlyEqual? UnitNames[i].Contains(lowerUnitName) : UnitNames[i].Any(lowerUnitName.Contains))
						return (SizeUnit)i;
				}

				throw new ArgumentOutOfRangeException(nameof(UnitName), "can't find unit named " + UnitName);
			}

			public FileSize(long BitsCount)
			{
				longSize = BitsCount;
				Size = BitsCount;
				Unit = 0;
				if (BitsCount < 8) return;
				Size /= 8;
				Unit++;
				for (; Size > 1024 && Unit<SizeUnit.YottaByte; Unit++)
				{
					Size /= 1024;
				}
			}

			private FileSize(double Size, SizeUnit Unit)
			{
				this.Size = Size;
				this.Unit = Unit;
				CalculateLongSize();
			}

			public static FileSize? Get(string Text)
			{
				if(Text.IsNullOrEmpty()) return null;

				int unitIndex = 0;
				while (Text[unitIndex].IsDoubleT())
				{
					unitIndex++;
					if (unitIndex == Text.Length) return null;
				}

				var size = Text[..unitIndex].ToDoubleT();
				var unit = getSizeUnit(Text[unitIndex..]);

				return new FileSize(size, unit);
			}
		}
	}
}
