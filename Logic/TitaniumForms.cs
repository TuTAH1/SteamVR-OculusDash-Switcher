using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

//Namespace contains library classes containing add-on to basic c# tools
namespace Titanium {
	/// <summary>
	/// Just my library of small functions that makes c# programming easier. The automatization of automatization instrument
	/// <para> Despite of the program license, THIS file is <see href="https://creativecommons.org/licenses/by-sa/4.0">CC BY-SA</see></para>
	/// <list type="table">
	/// <item>
	/// <term>Author</term>
	/// <see href="https://github.com/TuTAH1">Титан</see>
	/// </item>
	/// </list>
	/// </summary>
	public static class TypesFuncs { //!21.04.2022 


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

				public static string ToStringT<T>(this T[] Array, string Separator = "") //:21.04.2022 Added separator parametr, replaced [foreach] by [for]
				{
					string s = "";
					for (int i = 0; i < Array.Length; i++)
					{
						s += Array[i];
						if (i != Array.Length - 1) s += Separator;
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
					return S.ToLower() is "true" or "yes" or "да" or "1";
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

			#region List

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

							#endregion

		#endregion


		#region OtherTypeFuncs

			#region Int

			static int DivRem(int dividend, int divisor, out int reminder)
			{
				int quotient = dividend / divisor;
				reminder = dividend - divisor * quotient;
				return quotient;
			}

			#endregion

			#region String

			public static int SymbolsCount(this string s)
			{
				int i = s.Length;
				foreach (var c in s)
				{
					if (char.IsControl(c)) i--;
				}

				return i;
			}

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
						
			public static string FormatString(this string s, int n, Positon pos, char filler = ' ')
					{
						if (s.Length<n)
						{
							switch (pos)
							{
								case Positon.left:
								{
									for (int i = s.Length; i<n; i++)
									{
										s+=filler;
									}
								}
									break;
								case Positon.center:
								{
									int halfString = (n-s.Length)/2;
									for (int i = 0; i<halfString; i++)
									{
										s=s.Insert(0, filler.ToString());
									}
									for (int i = s.Length; i<n; i++)
									{
										s+=filler;
									}
								}
									break;
								case Positon.right:
								{
									for (int i = 0; i<(n-s.Length); i++)
									{
										s=s.Insert(0, filler.ToString());
									}
								}
									break;
							}
						}
						else if (s.Length>n)
						{
							int Eindex = s.LastIndexOf('E');

							if (Eindex>0)
							{ //если в строке есть Е+хх
								string E = s.TrimStart('E');
								s=s.Substring(0, n-E.Length);
								s+=E;
							}
							else
							{
								s=s.Substring(0, n);
							}
						}
						return s;
					}

			public enum Positon: byte { left,center,right}

			/// <summary>
			/// Slices the string form Start to End not including End
			/// </summary>
			/// <param name="s"></param>
			/// <param name="Start"></param>
			/// <param name="End"></param>
			/// <returns></returns>
			public static string Slice(this string s, int Start = 0, int End = Int32.MaxValue)
			{
				if (Start < 0) throw new ArgumentOutOfRangeException(nameof(Start),Start,null);
				if (Start > End) throw new ArgumentOutOfRangeException(nameof(Start),Start,$"start ({Start}) is be bigger than end ({End})");
				if (End > s.Length) End = s.Length;
				return s.Substring(Start, End - Start);
			}

			/// <summary>
			/// Обрезает строку, начиная с первого появления StartsWith и заканчивая последним появлением EndsWith
			/// </summary>
			/// <param name="s"></param>
			/// <param name="StartsWith">Строка, с которой будет происходить срезание</param>
			/// <param name="EndsWith">Строка, до которой будет происходить срезание</param>
			/// <param name="AlwaysReturnString">Возвращать строку, если начало или конец не найдены (иначе будет возвращен null)</param>
			/// <param name="LastStart">Начинать поиск Start с конца</param>
			/// <param name="LastEnd">Начинать поиск Start с конца</param>
			/// <returns></returns>
			public static string Slice(this string s, string StartsWith, string EndsWith, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true)
			{
				var start = StartsWith == null? 0 : LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
				if (start < 0) return  AlwaysReturnString? s : null;
				
				s = s.Slice(start);

				var end = EndsWith==null? s.Length-1 : LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
				if (end < 0) return  AlwaysReturnString? s : null;

				return s.Slice(0,end);
			}

			public static string SliceFromEnd(this string s, string StartsWith = null, string EndsWith = null, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true)
			{
				var end = EndsWith==null? s.Length-1 : LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
				if (end < 0) return  AlwaysReturnString? s : null;

				s = s.Slice(0, end);

				var start = StartsWith == null? 0 : LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
				if (start < 0) return  AlwaysReturnString? s : null;

				return s.Slice(start);
			}
			public static string Slice(this string s, int Start, string EndsWith, bool AlwaysReturnString = false, bool LastEnd = true)
			{
				var end = LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
				if (end < 0) return AlwaysReturnString? s : null;

				return s.Slice(Start, end);

			}
			public static string Slice(this string s, string StartsWith, int End = Int32.MaxValue, bool AlwaysReturnString = false, bool LastStart = false)
			{
				var start = LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
				if (start < 0) return  AlwaysReturnString? s : null;

				return s.Slice(start, End);

			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="s"></param>
			/// <param name="StartConditions">Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</param>
			/// <param name="EndConditions">Условия, которым должны удовлетворять символы конца строки (по функции на 1 символ)</param>
			/// <param name="AlwaysReturnString">Возвращать изначальную строку при неудачном поиске</param>
			/// <returns></returns>
			public static string Slice(this string s, Func<char,bool>[] StartConditions, Func<char,bool>[] EndConditions, bool AlwaysReturnString = false)
			{
				if (!StartConditions.Any()) throw new ArgumentNullException(nameof(StartConditions), "Условия начальной строки не заданы");
				if (!EndConditions.Any()) throw new ArgumentNullException(nameof(EndConditions), "Условия конечной строки не заданы");

				var start = s.IndexOfT(StartConditions,IndexOfEnd:true);
				if (start < 0) return  AlwaysReturnString? s : null;

				var end =s.IndexOfT(EndConditions,LastOccuarance:true);
				if (end < 0) return  AlwaysReturnString? s : null;

				return s.Slice(start, end);
			}

			public static string Slice(this string s, int Start, Func<char,bool>[] EndConditions, bool AlwaysReturnString = false)
			{
				if (!EndConditions.Any()) throw new ArgumentNullException(nameof(EndConditions), "Условия конечной строки не заданы");

				var end =s.IndexOfT(EndConditions,LastOccuarance:true);
				if (end < 0) return  AlwaysReturnString? s.Slice(Start) : null;

				return s.Slice(Start, end);
			}

			public static string Slice(this string s, int Start, Regex EndRegex, bool AlwaysReturnString = false)
			{
				var end = EndRegex.Match(s).Index;
				if (end < 0) return  AlwaysReturnString? s : null;

				return s.Slice(Start, end);
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
			/// Reports the position of a symbol next to last occurance of a string s2 (position after the end of s2) 
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

			public static int IndexOfT(this string s, Func<char,bool>[] Conditions, int Start = 0, int End = Int32.MaxValue, DirectionEnum Direction = DirectionEnum.Custom, bool IndexOfEnd = false, bool LastOccuarance = false)
			{
				if (End == Int32.MaxValue) End = Direction == DirectionEnum.Left ? 0 : s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) throw new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");

				if (End == Start) return -2;

				if (Direction == DirectionEnum.Custom)
					Direction = End > Start ? DirectionEnum.Right : DirectionEnum.Left;
				else if ((Direction == DirectionEnum.Left && End > Start) || (Direction == DirectionEnum.Right && End < Start)) 
					Swap(ref Start, ref End);

				bool rightDirection = Direction == DirectionEnum.Right;
				int defaultCurMatchPos = rightDirection? 0 : s.Length-1;
				int curCondition = defaultCurMatchPos;
				int Result = -1; 

				if (rightDirection)
					for (int i = Start; i < End; i++)
					{
						if (Conditions[curCondition](s[i]))
						{
							curCondition++;
							if (curCondition != Conditions.Length-1) continue;
							Result = i;
							curCondition = defaultCurMatchPos;
							if(!LastOccuarance) break;
						}
						else
						{
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
							if(!LastOccuarance) break;
						}
						else
						{
							curCondition = defaultCurMatchPos;
						}
					}

				return IndexOfEnd ? Result : Result - Conditions.Length;
			}

			public static int IndexOfT(this string s, string s2, int Start = 0, int End = Int32.MaxValue, DirectionEnum Direction = DirectionEnum.Custom, bool IndexOfEnd = false, bool LastOccuarance = false)
			{
				if (End == Int32.MaxValue) End = Direction == DirectionEnum.Left ? 0 : s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");

				if (End == Start) return -2;

				if (Direction == DirectionEnum.Custom)
					Direction = End > Start ? DirectionEnum.Right : DirectionEnum.Left;
				else if ((Direction == DirectionEnum.Left && End > Start) || (Direction == DirectionEnum.Right && End < Start)) 
					Swap(ref Start, ref End);

				bool rightDirection = Direction == DirectionEnum.Right;
				int defaultCurMatchPos = rightDirection? 0 : s.Length-1;
				int curMatchPos = defaultCurMatchPos;
				int Result = -1;

				if (rightDirection)
					for (int i = Start; i < End; i++)
					{
						if (s[i] == s2[curMatchPos])
						{
							curMatchPos++;
							if (curMatchPos != s2.Length) continue;
							Result = i;
							curMatchPos = defaultCurMatchPos;
							if(!LastOccuarance) break;
						}
						else
						{
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
							if(!LastOccuarance) break;
						}
						else
						{
							curMatchPos = defaultCurMatchPos;
						}
					}

				return IndexOfEnd? Result : Result - s2.Length;
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
			/// s[..Start] + NewString + s[End..]
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

			/// <summary>
			/// Adds unadded part of <paramref name="addiction"/> to <paramref name="s"/> in the end of <param name="s"/>. <c>"stringTest".Add("Testing") //"stringTesting"</c> <paramref name="s"/> will be unchanged if <paramref name="addiction"/> is already added to <paramref name="s"/> <c>"Test".Add("st") //"Test"</c>. Replacement occuts only in end <c>"Tests".Add("Testing") //"TestsTesting"</c>
			/// </summary>
			/// <param name="s">Initial string</param>
			/// <param name="addiction">String that should be in the end of <paramref name="s"/></param>
			/// <returns></returns>
			public static string Add(this string s, string addiction)
			{
				int offset = 0;

				//:Searches for *addiction* in the end of the *s*
				for (; offset < addiction.Length; offset++)
				{
					if (s[^1] != addiction[offset]) continue; 
					int aPosition = offset;
					for (int sPosition = s.Length-1; sPosition >0;)
					{
						if (s[sPosition--] != addiction[aPosition--]) break;
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

			#region Array

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

			public static string ToStringLine<T>(this T[] Array, string Separator = " ")
			{
				string result = "";
				foreach (var el in Array)
				{
					result += el + Separator;
				}

				return result[..^Separator.Length];
			}

			public static string ToStringLine<T>(this List<T> Array, string Separator = " ")
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

			public static string Remove(this string S, string RemovableString, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				if (RemovableString is null or "") return S;
				int startPos = S.IndexOf(RemovableString, ComparisonType);
				return startPos == -1 ? S : S.Remove(startPos, RemovableString.Length);
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

			#region Image

			
			public static Image Resize(this Image i, int NewDimensionValue, Dimension FixedDimension = Dimension.Height)
			{
				return i == null? null : new Bitmap(i, i.Size.Resize(NewDimensionValue, FixedDimension));
			}
			#endregion

			#region Color

			static Color Change(this Color c, byte? A = null, byte? R = null, byte? G = null, byte? B = null) => Color.FromArgb(A?? c.A, R??c.R, G??c.G, B??c.B);

			#endregion

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

	}
}
