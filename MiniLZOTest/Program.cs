using System;
using System.Diagnostics;

namespace MiniLZOTest {
	class Program {
		static void Main(string[] args) {
			Random r = new Random();
			for (int i = 0; i < 1000; i++) {
				var input = new byte[r.Next(1000000)];
				byte[] compressed = MiniLZO.MiniLZO.Compress(input);
				byte[] decompressed = new byte[input.Length];
				MiniLZO.MiniLZO.Decompress(compressed, decompressed);
				AssertEqual(input, decompressed);
			}
		}

		static void AssertEqual(byte[] l, byte[] r) {
			Debug.Assert(l.Length == r.Length);
			for (int i = 0; i < l.Length; i++)
				Debug.Assert(l[i] == r[i]);
		}

	}
}
