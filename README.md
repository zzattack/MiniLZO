MiniLZO
=======
This is a C# port of the LZO real-time data compression library as found on http://www.oberhumer.com/opensource/lzo/.

It is really a copy-paste of the original code after putting it through the MSVC preprocessor,
changing only the used datatypes to their .NET equivalents and fixing up some control flow to
amend for the semantic differences between C's and C#'s goto statement.

To add insult to injury, the sources I used were of the 1.07 version while for a while the 2.06 
version has been out. If at all possible, avoid using this port.
