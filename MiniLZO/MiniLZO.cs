
#region Copyright notice
/* C# port of the crude minilzo source version 1.07 by Frank Razenberg
 
  Beware, you should never want to see C# code like this. You were warned.
  I simply ran the MSVC preprocessor on the original 1.07 source, changed the datatypes 
  to their C# counterpart and fixed changed some control flow stuff to amend for
  the different goto semantics between C and C#.

  Original copyright notice is included below.
*/


/* 
   This file is part of the LZO real-time data compression library.

   Copyright (C) 2001 Markus Franz Xaver Johannes Oberhumer
   Copyright (C) 2000 Markus Franz Xaver Johannes Oberhumer
   Copyright (C) 1999 Markus Franz Xaver Johannes Oberhumer
   Copyright (C) 1998 Markus Franz Xaver Johannes Oberhumer
   Copyright (C) 1997 Markus Franz Xaver Johannes Oberhumer
   Copyright (C) 1996 Markus Franz Xaver Johannes Oberhumer
   All Rights Reserved.

   The LZO library is free software; you can redistribute it and/or
   modify it under the terms of the GNU General Public License,
   version 2, as published by the Free Software Foundation.

   The LZO library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with the LZO library; see the file COPYING.
   If not, write to the Free Software Foundation, Inc.,
   51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.

   Markus F.X.J. Oberhumer
   <markus@oberhumer.com>
   http://www.oberhumer.com/opensource/lzo/
 */

/*
 * NOTE:
 *   the full LZO package can be found at
 *   http://www.oberhumer.com/opensource/lzo/
 */

#endregion


namespace MiniLZO {
	using System;

	public static class MiniLZO {

		public static unsafe int lzo1x_1_compress(byte* @in, uint in_len, byte* @out, ref uint out_len, byte* wrkmem) {
			byte* ip = @in;
			byte* op = @out;
			uint l = in_len;
			uint t = 0;

			if (in_len <= 8 + 5)
				t = in_len;
			else {
				t = lzo1x_1_do_compress(@in, in_len, op, ref out_len, wrkmem);
				op += out_len;
			}
			if (t > 0) {
				byte* ii = @in + in_len - t;
				if (op == @out && t <= 238)
					*op++ = ((byte)(17 + t));
				else if (t <= 3)
					op[-2] |= ((byte)(t));
				else if (t <= 18)
					*op++ = ((byte)(t - 3));
				else {
					uint tt = t - 18;
					*op++ = 0;
					while (tt > 255) {
						tt -= 255;
						*op++ = 0;
					}
					*op++ = ((byte)(tt));
				}
				do *op++ = *ii++; while (--t > 0);
			}
			*op++ = 16 | 1;
			*op++ = 0;
			*op++ = 0;
			out_len = (uint)(op - @out);
			return 0;
		}

		private static unsafe uint lzo1x_1_do_compress(byte* @in, uint in_len, byte* @out, ref uint out_len, byte* wrkmem) {
			byte* ip;
			byte* op;
			byte* in_end = @in + in_len;
			byte* ip_end = @in + in_len - 8 - 5;
			byte* ii;
			byte** dict = (byte**)wrkmem;
			op = @out;
			ip = @in;
			ii = ip;
			ip += 4;
			for (; ; ) {
				byte* m_pos;
				uint m_off;
				uint m_len;
				uint dindex;
				dindex =
					((uint)
						((((0x21 * (((((((uint)(((ip) + 1)[2]) << (6)) ^ ((ip) + 1)[1]) << (5)) ^ ((ip) + 1)[0]) << (5)) ^ (ip)[0])) >> 5) &
						(((1u << (14)) - 1) >> (0))) << (0)));
				m_pos = dict[dindex];
				if (((((((ulong)(m_pos)) < ((ulong)(@in))) || (m_off = (uint)((int)(((ulong)(ip)) - ((ulong)(m_pos))))) <= 0 ||
						m_off > 0xbfff))))
					goto literal;
				if (m_off <= 0x0800 || m_pos[3] == ip[3])
					goto try_match;
				dindex = (dindex & (((1u << (14)) - 1) & 0x7ff)) ^ (((((1u << (14)) - 1) >> 1) + 1) | 0x1f);
				m_pos = dict[dindex];
				if (((((((ulong)(m_pos)) < ((ulong)(@in))) || (m_off = (uint)((int)(((ulong)(ip)) - ((ulong)(m_pos))))) <= 0 ||
						m_off > 0xbfff))))
					goto literal;
				if (m_off <= 0x0800 || m_pos[3] == ip[3])
					goto try_match;
				goto literal;
			try_match:
				if (*(ushort*)m_pos != *(ushort*)ip) {
				}
				else {
					if (m_pos[2] == ip[2]) {
						goto match;
					}
					else {
					}
				}
			literal:
				dict[dindex] = (ip);
				++ip;
				if (ip >= ip_end)
					break;
				continue;
			match:
				dict[dindex] = (ip);
				if (ip - ii > 0) {
					uint t = (uint)ip - (uint)ii;
					if (t <= 3) {

						op[-2] |= ((byte)(t));
					}
					else if (t <= 18)
						*op++ = ((byte)(t - 3));
					else {
						uint tt = t - 18;
						*op++ = 0;
						while (tt > 255) {
							tt -= 255;
							*op++ = 0;
						}

						*op++ = ((byte)(tt));
					}
					do *op++ = *ii++; while (--t > 0);
				}

				ip += 3;
				if (m_pos[3] != *ip++ || m_pos[4] != *ip++ || m_pos[5] != *ip++ ||
					m_pos[6] != *ip++ || m_pos[7] != *ip++ || m_pos[8] != *ip++
					) {
					--ip;
					m_len = (uint)ip - (uint)ii;

					if (m_off <= 0x0800) {
						m_off -= 1;
						*op++ = ((byte)(((m_len - 1) << 5) | ((m_off & 7) << 2)));
						*op++ = ((byte)(m_off >> 3));
					}
					else if (m_off <= 0x4000) {
						m_off -= 1;
						*op++ = ((byte)(32 | (m_len - 2)));
						m3_m4_offset(ref op, m_off);
					}
					else {
						m_off -= 0x4000;

						*op++ = ((byte)(16 | ((m_off & 0x4000) >> 11) | (m_len - 2)));
						m3_m4_offset(ref op, m_off);
					}
				}
				else {
					{
						byte* end = in_end;
						byte* m = m_pos + 8 + 1;
						while (ip < end && *m == *ip) {
							m++;
							ip++;
						}
						m_len = (uint)(ip - ii);
					}

					if (m_off <= 0x4000) {
						m_off -= 1;
						if (m_len <= 33)
							*op++ = ((byte)(32 | (m_len - 2)));
						else {
							m_len -= 33;
							*op++ = 32 | 0;
							m3_m4_len(m_len, ref op);
						}
					}
					else {
						m_off -= 0x4000;

						if (m_len <= 9)
							*op++ = ((byte)(16 | ((m_off & 0x4000) >> 11) | (m_len - 2)));
						else {
							m_len -= 9;
							*op++ = ((byte)(16 | ((m_off & 0x4000) >> 11)));
						m3_m4_len:
							m3_m4_len(m_len, ref op);
						}
					}
					m3_m4_offset(ref op, m_off);
				}
				ii = ip;
				if (ip >= ip_end)
					break;
			}
			out_len = (uint)(op - @out);
			return (uint)(in_end - ii);
		}

		public static unsafe int lzo1x_decompress(byte* @in, uint in_len, byte* @out, ref uint out_len, byte* wrkmem) {
			byte* op;
			byte* ip;
			uint t;
			byte* m_pos;
			byte* ip_end = @in + in_len;
			out_len = 0;
			op = @out;
			ip = @in;
			bool gt_first_literal_run = false;
			if (*ip > 17) {
				t = (uint)(*ip++ - 17);
				if (t < 4)
					match_next(ref op, ref ip, ref t);
				else {
					do *op++ = *ip++; while (--t > 0);
					gt_first_literal_run = true;
				}
			}

			while (true) {
				if (gt_first_literal_run) {
					gt_first_literal_run = false;
					goto first_literal_run;
				}

				t = *ip++;
				if (t >= 16)
					goto match;
				if (t == 0) {

					while (*ip == 0) {
						t += 255;
						ip++;
					}
					t += (uint)(15 + *ip++);
				}

				*(uint*)op = *(uint*)ip;
				op += 4;
				ip += 4;
				if (--t > 0) {
					if (t >= 4) {
						do {
							*(uint*)op = *(uint*)ip;
							op += 4;
							ip += 4;
							t -= 4;
						} while (t >= 4);
						if (t > 0) do *op++ = *ip++; while (--t > 0);
					}
					else
						do *op++ = *ip++; while (--t > 0);
				}
			first_literal_run:
				t = *ip++;
				if (t >= 16)
					goto match;
				m_pos = op - (1 + 0x0800);
				m_pos -= t >> 2;
				m_pos -= *ip++ << 2;

				*op++ = *m_pos++;
				*op++ = *m_pos++;
				*op++ = *m_pos;
				// goto match_done;
				t = (uint)(ip[-2] & 3);
				if (t == 0)
					break;
				else {
					match_next(ref op, ref ip, ref t);
					continue;
				}
			// end goto match_done

				match:
				while (true) {
					if (t >= 64) {
						m_pos = op - 1;
						m_pos -= (t >> 2) & 7;
						m_pos -= *ip++ << 3;
						t = (t >> 5) - 1;

						copy_match(ref op, ref m_pos, ref t);
						goto match_done;
					}
					else if (t >= 32) {
						t &= 31;
						if (t == 0) {

							while (*ip == 0) {
								t += 255;
								ip++;

							}
							t += (uint)(31 + *ip++);
						}
						m_pos = op - 1;
						m_pos -= (*(ushort*)ip) >> 2;
						ip += 2;
					}
					else if (t >= 16) {
						m_pos = op;
						m_pos -= (t & 8) << 11;
						t &= 7;
						if (t == 0) {
							while (*ip == 0) {
								t += 255;
								ip++;
							}
							t += (uint)(7 + *ip++);
						}
						m_pos -= (*(ushort*)ip) >> 2;
						ip += 2;
						if (m_pos == op)
							goto eof_found;
						m_pos -= 0x4000;
					}
					else {
						m_pos = op - 1;
						m_pos -= t >> 2;
						m_pos -= *ip++ << 2;

						*op++ = *m_pos++;
						*op++ = *m_pos;
						goto match_done;
					}

					if (t >= 2 * 4 - (3 - 1) && (op - m_pos) >= 4) {
						*(uint*)op = *(uint*)m_pos;
						op += 4;
						m_pos += 4;
						t -= 4 - (3 - 1);
						do {
							*(uint*)op = *(uint*)m_pos;
							op += 4;
							m_pos += 4;
							t -= 4;
						} while (t >= 4);
						if (t > 0) do *op++ = *m_pos++; while (--t > 0);
					}
					else {
					copy_match:
						copy_match(ref op, ref m_pos, ref t);
					}
				match_done:
					t = (uint)(ip[-2] & 3);
					if (t == 0)
						break;
				match_next:
					match_next(ref op, ref ip, ref t);
				}
			}

		eof_found:
			out_len = (uint)(op - @out);
			return (ip == ip_end
				? 0
				: (ip < ip_end ? (-8) : (-4)));
		}

		private static unsafe void copy_match(ref byte* op, ref byte* m_pos, ref uint t) {
			*op++ = *m_pos++;
			*op++ = *m_pos++;
			do *op++ = *m_pos++; while (--t > 0);
		}

		private static unsafe void match_next(ref byte* op, ref byte* ip, ref uint t) {
			do *op++ = *ip++; while (--t > 0);
			t = *ip++;
		}

		private static unsafe void m3_m4_len(uint m_len, ref byte* op) {
			while (m_len > 255) {
				m_len -= 255;
				*op++ = 0;
			}
			*op++ = ((byte)(m_len));
		}

		private static unsafe void m3_m4_offset(ref byte* op, uint m_off) {
			*op++ = ((byte)((m_off & 63) << 2));
			*op++ = ((byte)(m_off >> 6));
		}

		public static unsafe byte[] Decompress(byte[] @in, byte[] @out) {
			uint out_len = 0;
			fixed (byte* @pIn = @in, wrkmem = new byte[IntPtr.Size * 16384], pOut = @out) {
				lzo1x_decompress(pIn, (uint)@in.Length, @pOut, ref @out_len, wrkmem);
			}
			return @out;
		}

		public static unsafe void Decompress(byte* r, uint size_in, byte* w, ref uint size_out) {
			fixed (byte* wrkmem = new byte[IntPtr.Size * 16384]) {
				lzo1x_decompress(r, size_in, w, ref size_out, wrkmem);
			}
		}

		public static unsafe byte[] Compress(byte[] input) {
			byte[] @out = new byte[input.Length + (input.Length / 16) + 64 + 3];
			uint out_len = 0;
			fixed (byte* @pIn = input, wrkmem = new byte[IntPtr.Size * 16384], pOut = @out) {
				lzo1x_1_compress(pIn, (uint)input.Length, @pOut, ref @out_len, wrkmem);
			}
			Array.Resize(ref @out, (int)out_len);
			return @out;
		}

		public static unsafe void Compress(byte* r, uint size_in, byte* w, ref uint size_out) {
			fixed (byte* wrkmem = new byte[IntPtr.Size * 16384]) {
				lzo1x_1_compress(r, size_in, w, ref size_out, wrkmem);
			}
		}
	}
}
