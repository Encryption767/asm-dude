﻿using System;

namespace AsmTools {

    public static class RegisterTools {

        public static Tuple<bool, Rn, int> toRn(string str) {
            Rn rn = parseRn(str);
            if (rn == Rn.NOREG) {
                return new Tuple<bool, Rn, int>(false, Rn.NOREG, 0);
            } else {
                return new Tuple<bool, Rn, int>(true, rn, nBits(rn));
            }
        }

        public static Rn parseRn(string str) {
            switch (str.ToUpper()) {
                case "RAX": return Rn.rax;
                case "EAX": return Rn.eax;
                case "AX": return Rn.ax;
                case "AL": return Rn.al;
                case "AH": return Rn.ah;

                case "RBX": return Rn.rbx;
                case "EBX": return Rn.ebx;
                case "BX": return Rn.bx;
                case "BL": return Rn.bl;
                case "BH": return Rn.bh;

                case "RCX": return Rn.rcx;
                case "ECX": return Rn.ecx;
                case "CX": return Rn.cx;
                case "CL": return Rn.cl;
                case "CH": return Rn.ch;

                case "RDX": return Rn.rdx;
                case "EDX": return Rn.edx;
                case "DX": return Rn.dx;
                case "DL": return Rn.dl;
                case "DH": return Rn.dh;

                case "RSI": return Rn.rsi;
                case "ESI": return Rn.esi;
                case "SI": return Rn.si;
                case "SIL": return Rn.sil;

                case "RDI": return Rn.rdi;
                case "EDI": return Rn.edi;
                case "DI": return Rn.di;
                case "DIL": return Rn.dil;

                case "RBP": return Rn.rbp;
                case "EBP": return Rn.ebp;
                case "BP": return Rn.bp;
                case "BPL": return Rn.bpl;

                case "RSP": return Rn.rsp;
                case "ESP": return Rn.esp;
                case "SP": return Rn.sp;
                case "SPL": return Rn.spl;

                case "R8": return Rn.R8;
                case "R8D": return Rn.R8D;
                case "R8W": return Rn.R8W;
                case "R8B": return Rn.R8B;

                case "R9": return Rn.R9;
                case "R9D": return Rn.R9D;
                case "R9W": return Rn.R9W;
                case "R9B": return Rn.R9B;

                case "R10": return Rn.R10;
                case "R10D": return Rn.R10D;
                case "R10W": return Rn.R10W;
                case "R10B": return Rn.R10B;

                case "R11": return Rn.R11;
                case "R11D": return Rn.R11D;
                case "R11W": return Rn.R11W;
                case "R11B": return Rn.R11B;

                case "R12": return Rn.R12;
                case "R12D": return Rn.R12D;
                case "R12W": return Rn.R12W;
                case "R12B": return Rn.R12B;

                case "R13": return Rn.R13;
                case "R13D": return Rn.R13D;
                case "R13W": return Rn.R13W;
                case "R13B": return Rn.R13B;

                case "R14": return Rn.R14;
                case "R14D": return Rn.R14D;
                case "R14W": return Rn.R14W;
                case "R14B": return Rn.R14B;

                case "R15": return Rn.R15;
                case "R15D": return Rn.R15D;
                case "R15W": return Rn.R15W;
                case "R15B": return Rn.R15B;

                case "MM0": return Rn.MM0;
                case "MM1": return Rn.MM1;
                case "MM2": return Rn.MM2;
                case "MM3": return Rn.MM3;
                case "MM4": return Rn.MM4;
                case "MM5": return Rn.MM5;
                case "MM6": return Rn.MM6;
                case "MM7": return Rn.MM7;

                case "XMM0": return Rn.XMM0;
                case "XMM1": return Rn.XMM1;
                case "XMM2": return Rn.XMM2;
                case "XMM3": return Rn.XMM3;
                case "XMM4": return Rn.XMM4;
                case "XMM5": return Rn.XMM5;
                case "XMM6": return Rn.XMM6;
                case "XMM7": return Rn.XMM7;

                case "XMM8": return Rn.XMM8;
                case "XMM9": return Rn.XMM9;
                case "XMM10": return Rn.XMM10;
                case "XMM11": return Rn.XMM11;
                case "XMM12": return Rn.XMM12;
                case "XMM13": return Rn.XMM13;
                case "XMM14": return Rn.XMM14;
                case "XMM15": return Rn.XMM15;

                case "YMM0": return Rn.YMM0;
                case "YMM1": return Rn.YMM1;
                case "YMM2": return Rn.YMM2;
                case "YMM3": return Rn.YMM3;
                case "YMM4": return Rn.YMM4;
                case "YMM5": return Rn.YMM5;
                case "YMM6": return Rn.YMM6;
                case "YMM7": return Rn.YMM7;

                case "YMM8": return Rn.YMM8;
                case "YMM9": return Rn.YMM9;
                case "YMM10": return Rn.YMM10;
                case "YMM11": return Rn.YMM11;
                case "YMM12": return Rn.YMM12;
                case "YMM13": return Rn.YMM13;
                case "YMM14": return Rn.YMM14;
                case "YMM15": return Rn.YMM15;

                case "ZMM0": return Rn.ZMM0;
                case "ZMM1": return Rn.ZMM1;
                case "ZMM2": return Rn.ZMM2;
                case "ZMM3": return Rn.ZMM3;
                case "ZMM4": return Rn.ZMM4;
                case "ZMM5": return Rn.ZMM5;
                case "ZMM6": return Rn.ZMM6;
                case "ZMM7": return Rn.ZMM7;

                case "ZMM8": return Rn.ZMM8;
                case "ZMM9": return Rn.ZMM9;
                case "ZMM10": return Rn.ZMM10;
                case "ZMM11": return Rn.ZMM11;
                case "ZMM12": return Rn.ZMM12;
                case "ZMM13": return Rn.ZMM13;
                case "ZMM14": return Rn.ZMM14;
                case "ZMM15": return Rn.ZMM15;

                case "ZMM16": return Rn.ZMM16;
                case "ZMM17": return Rn.ZMM17;
                case "ZMM18": return Rn.ZMM18;
                case "ZMM19": return Rn.ZMM19;
                case "ZMM20": return Rn.ZMM20;
                case "ZMM21": return Rn.ZMM21;
                case "ZMM22": return Rn.ZMM22;
                case "ZMM23": return Rn.ZMM23;

                case "ZMM24": return Rn.ZMM24;
                case "ZMM25": return Rn.ZMM25;
                case "ZMM26": return Rn.ZMM26;
                case "ZMM27": return Rn.ZMM27;
                case "ZMM28": return Rn.ZMM28;
                case "ZMM29": return Rn.ZMM29;
                case "ZMM30": return Rn.ZMM30;
                case "ZMM31":
                    return Rn.ZMM31;
                default:
                    return Rn.NOREG;
            }
        }

        public static int nBits(Rn rn) {
            switch (rn) {
                case Rn.rax:
                case Rn.rbx:
                case Rn.rcx:
                case Rn.rdx:
                case Rn.rsi:
                case Rn.rdi:
                case Rn.rbp:
                case Rn.rsp:
                case Rn.R8:
                case Rn.R9:
                case Rn.R10:
                case Rn.R11:
                case Rn.R12:
                case Rn.R13:
                case Rn.R14:
                case Rn.R15:
                    return 64;

                case Rn.eax:
                case Rn.ebx:
                case Rn.ecx:
                case Rn.edx:
                case Rn.esi:
                case Rn.edi:
                case Rn.ebp:
                case Rn.esp:
                case Rn.R8D:
                case Rn.R9D:
                case Rn.R10D:
                case Rn.R11D:
                case Rn.R12D:
                case Rn.R13D:
                case Rn.R14D:
                case Rn.R15D:
                    return 32;

                case Rn.ax:
                case Rn.bx:
                case Rn.cx:
                case Rn.dx:
                case Rn.si:
                case Rn.di:
                case Rn.bp:
                case Rn.sp:
                case Rn.R8W:
                case Rn.R9W:
                case Rn.R10W:
                case Rn.R11W:
                case Rn.R12W:
                case Rn.R13W:
                case Rn.R14W:
                case Rn.R15W:
                    return 16;

                case Rn.al:
                case Rn.bl:
                case Rn.cl:
                case Rn.dl:
                case Rn.ah:
                case Rn.bh:
                case Rn.ch:
                case Rn.dh:
                case Rn.sil:
                case Rn.dil:
                case Rn.bpl:
                case Rn.spl:
                case Rn.R8B:
                case Rn.R9B:
                case Rn.R10B:
                case Rn.R11B:
                case Rn.R12B:
                case Rn.R13B:
                case Rn.R14B:
                case Rn.R15B:
                    return 8;

                case Rn.MM0:
                case Rn.MM1:
                case Rn.MM2:
                case Rn.MM3:
                case Rn.MM4:
                case Rn.MM5:
                case Rn.MM6:
                case Rn.MM7:
                    return 64;

                case Rn.XMM0:
                case Rn.XMM1:
                case Rn.XMM2:
                case Rn.XMM3:
                case Rn.XMM4:
                case Rn.XMM5:
                case Rn.XMM6:
                case Rn.XMM7:
                case Rn.XMM8:
                case Rn.XMM9:
                case Rn.XMM10:
                case Rn.XMM11:
                case Rn.XMM12:
                case Rn.XMM13:
                case Rn.XMM14:
                case Rn.XMM15:
                    return 128;

                case Rn.YMM0:
                case Rn.YMM1:
                case Rn.YMM2:
                case Rn.YMM3:
                case Rn.YMM4:
                case Rn.YMM5:
                case Rn.YMM6:
                case Rn.YMM7:
                case Rn.YMM8:
                case Rn.YMM9:
                case Rn.YMM10:
                case Rn.YMM11:
                case Rn.YMM12:
                case Rn.YMM13:
                case Rn.YMM14:
                case Rn.YMM15:
                    return 256;

                case Rn.ZMM0:
                case Rn.ZMM1:
                case Rn.ZMM2:
                case Rn.ZMM3:
                case Rn.ZMM4:
                case Rn.ZMM5:
                case Rn.ZMM6:
                case Rn.ZMM7:
                case Rn.ZMM8:
                case Rn.ZMM9:
                case Rn.ZMM10:
                case Rn.ZMM11:
                case Rn.ZMM12:
                case Rn.ZMM13:
                case Rn.ZMM14:
                case Rn.ZMM15:

                case Rn.ZMM16:
                case Rn.ZMM17:
                case Rn.ZMM18:
                case Rn.ZMM19:
                case Rn.ZMM20:
                case Rn.ZMM21:
                case Rn.ZMM22:
                case Rn.ZMM23:
                case Rn.ZMM24:
                case Rn.ZMM25:
                case Rn.ZMM26:
                case Rn.ZMM27:
                case Rn.ZMM28:
                case Rn.ZMM29:
                case Rn.ZMM30:
                case Rn.ZMM31:
                    return 512;
            }
            return 0;
        }

        /// <summary>
        /// return regular pattern to select the provided register and aliased register names
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static string getRelatedRegister(string reg) {

            //TODO use register enum
            switch (reg.ToUpper()) {
                case "RAX":
                case "EAX":
                case "AX":
                case "AL":
                case "AH":
                    return "\\b(RAX|EAX|AX|AH|AL)\\b";
                case "RBX":
                case "EBX":
                case "BX":
                case "BL":
                case "BH":
                    return "\\b(RBX|EBX|BX|BH|BL)\\b";
                case "RCX":
                case "ECX":
                case "CX":
                case "CL":
                case "CH":
                    return "\\b(RCX|ECX|CX|CH|CL)\\b";
                case "RDX":
                case "EDX":
                case "DX":
                case "DL":
                case "DH":
                    return "\\b(RDX|EDX|DX|DH|DL)\\b";
                case "RSI":
                case "ESI":
                case "SI":
                case "SIL":
                    return "\\b(RSI|ESI|SI|SIL)\\b";
                case "RDI":
                case "EDI":
                case "DI":
                case "DIL":
                    return "\\b(RDI|EDI|DI|DIL)\\b";
                case "RBP":
                case "EBP":
                case "BP":
                case "BPL":
                    return "\\b(RBP|EBP|BP|BPL)\\b";
                case "RSP":
                case "ESP":
                case "SP":
                case "SPL":
                    return "\\b(RSP|ESP|SP|SPL)\\b";
                case "R8":
                case "R8D":
                case "R8W":
                case "R8B":
                    return "\\b(R8|R8D|R8W|R8B)\\b";
                case "R9":
                case "R9D":
                case "R9W":
                case "R9B":
                    return "\\b(R9|R9D|R9W|R9B)\\b";
                case "R10":
                case "R10D":
                case "R10W":
                case "R10B":
                    return "\\b(R10|R10D|R10W|R10B)\\b";
                case "R11":
                case "R11D":
                case "R11W":
                case "R11B":
                    return "\\b(R11|R11D|R11W|R11B)\\b";
                case "R12":
                case "R12D":
                case "R12W":
                case "R12B":
                    return "\\b(R12|R12D|R12W|R12B)\\b";
                case "R13":
                case "R13D":
                case "R13W":
                case "R13B":
                    return "\\b(R13|R13D|R13W|R13B)\\b";
                case "R14":
                case "R14D":
                case "R14W":
                case "R14B":
                    return "\\b(R14|R14D|R14W|R14B)\\b";
                case "R15":
                case "R15D":
                case "R15W":
                case "R15B":
                    return "\\b(R15|R15D|R15W|R15B)\\b";

                case "XMM0":
                case "YMM0":
                case "ZMM0":
                    return "\\b(XMM0|YMM0|ZMM0)\\b";

                case "XMM1":
                case "YMM1":
                case "ZMM1":
                    return "\\b(XMM1|YMM1|ZMM1)\\b";
                case "XMM2":
                case "YMM2":
                case "ZMM2":
                    return "\\b(XMM2|YMM2|ZMM2)\\b";
                case "XMM3":
                case "YMM3":
                case "ZMM3":
                    return "\\b(XMM3|YMM3|ZMM3)\\b";
                case "XMM4":
                case "YMM4":
                case "ZMM4":
                    return "\\b(XMM4|YMM4|ZMM4)\\b";
                case "XMM5":
                case "YMM5":
                case "ZMM5":
                    return "\\b(XMM5|YMM5|ZMM5)\\b";
                case "XMM6":
                case "YMM6":
                case "ZMM6":
                    return "\\b(XMM6|YMM6|ZMM6)\\b";
                case "XMM7":
                case "YMM7":
                case "ZMM7":
                    return "\\b(XMM7|YMM7|ZMM7)\\b";
                case "XMM8":
                case "YMM8":
                case "ZMM8":
                    return "\\b(XMM8|YMM8|ZMM8)\\b";
                case "XMM9":
                case "YMM9":
                case "ZMM9":
                    return "\\b(XMM9|YMM9|ZMM9)\\b";
                case "XMM10":
                case "YMM10":
                case "ZMM10":
                    return "\\b(XMM10|YMM10|ZMM10)\\b";
                case "XMM11":
                case "YMM11":
                case "ZMM11":
                    return "\\b(XMM11|YMM11|ZMM11)\\b";
                case "XMM12":
                case "YMM12":
                case "ZMM12":
                    return "\\b(XMM12|YMM12|ZMM12)\\b";
                case "XMM13":
                case "YMM13":
                case "ZMM13":
                    return "\\b(XMM13|YMM13|ZMM13)\\b";
                case "XMM14":
                case "YMM14":
                case "ZMM14":
                    return "\\b(XMM14|YMM14|ZMM14)\\b";
                case "XMM15":
                case "YMM15":
                case "ZMM15":
                    return "\\b(XMM15|YMM15|ZMM15)\\b";

                default: return reg;
            }
        }


        private static bool isRegisterMethod1(string keyword) {
            //TODO  get this info from AsmDudeData.xml
            switch (keyword.ToUpper()) {

                #region GPR
                case "RAX"://
                case "EAX"://
                case "AX"://
                case "AL"://
                case "AH"://

                case "RBX"://
                case "EBX"://
                case "BX"://
                case "BL"://
                case "BH"://

                case "RCX"://
                case "ECX"://
                case "CX"://
                case "CL"://
                case "CH"://

                case "RDX"://
                case "EDX"://
                case "DX"://
                case "DL"://
                case "DH"://

                case "RSI"://
                case "ESI"://
                case "SI"://
                case "SIL"://

                case "RDI"://
                case "EDI"://
                case "DI"://
                case "DIL"://

                case "RBP"://
                case "EBP"://
                case "BP"://
                case "BPL"://

                case "RSP"://
                case "ESP"://
                case "SP"://
                case "SPL"://

                case "R8"://
                case "R8D"://
                case "R8W"://
                case "R8B"://

                case "R9"://
                case "R9D"://
                case "R9W"://
                case "R9B"://

                case "R10"://
                case "R10D"://
                case "R10W"://
                case "R10B"://

                case "R11"://
                case "R11D"://
                case "R11W"://
                case "R11B"://

                case "R12"://
                case "R12D"://
                case "R12W"://
                case "R12B"://

                case "R13"://
                case "R13D"://
                case "R13W"://
                case "R13B"://

                case "R14"://
                case "R14D"://
                case "R14W"://
                case "R14B"://

                case "R15"://
                case "R15D"://
                case "R15W"://
                case "R15B"://

                #endregion GPR
                #region SIMD

                case "MM0"://
                case "MM1"://
                case "MM2"://
                case "MM3"://
                case "MM4"://
                case "MM5"://
                case "MM6"://
                case "MM7"://

                case "XMM0"://
                case "XMM1"://
                case "XMM2"://
                case "XMM3"://
                case "XMM4"://
                case "XMM5"://
                case "XMM6"://
                case "XMM7"://

                case "XMM8"://
                case "XMM9"://
                case "XMM10"://
                case "XMM11"://
                case "XMM12"://
                case "XMM13"://
                case "XMM14"://
                case "XMM15"://

                case "YMM0"://
                case "YMM1"://
                case "YMM2"://
                case "YMM3"://
                case "YMM4"://
                case "YMM5"://
                case "YMM6"://
                case "YMM7"://

                case "YMM8"://
                case "YMM9"://
                case "YMM10"://
                case "YMM11"://
                case "YMM12"://
                case "YMM13"://
                case "YMM14"://
                case "YMM15"://

                case "ZMM0"://
                case "ZMM1"://
                case "ZMM2"://
                case "ZMM3"://
                case "ZMM4"://
                case "ZMM5"://
                case "ZMM6"://
                case "ZMM7"://

                case "ZMM8"://
                case "ZMM9"://
                case "ZMM10":
                case "ZMM11":
                case "ZMM12":
                case "ZMM13":
                case "ZMM14":
                case "ZMM15":

                case "ZMM16":
                case "ZMM17":
                case "ZMM18":
                case "ZMM19":
                case "ZMM20":
                case "ZMM21":
                case "ZMM22":
                case "ZMM23":

                case "ZMM24":
                case "ZMM25":
                case "ZMM26":
                case "ZMM27":
                case "ZMM28":
                case "ZMM29":
                case "ZMM30":
                case "ZMM31":
                    #endregion SIMD
                    return true;
                default:
                    return false;
            }
        }

        private static bool isRegisterMethod2(string keyword) {
            int length = keyword.Length;
            string str = keyword.ToUpper();
            char c1 = str[0];
            char c2 = str[1];
            char c3 = (length > 2) ? str[2] : ' ';
            char c4 = (length > 3) ? str[3] : ' ';
            char c5 = (length > 4) ? str[4] : ' ';

            switch (length) {
                #region length2
                case 2:
                    switch (c1) {
                        case 'A': return (c2 == 'X') || (c2 == 'H') || (c2 == 'L');
                        case 'B': return (c2 == 'X') || (c2 == 'H') || (c2 == 'L') || (c2 == 'P');
                        case 'C': return (c2 == 'X') || (c2 == 'H') || (c2 == 'L');
                        case 'D': return (c2 == 'X') || (c2 == 'H') || (c2 == 'L') || (c2 == 'I');
                        case 'S': return (c2 == 'I') || (c2 == 'P');
                        case 'R': return (c2 == '8') || (c2 == '9');
                    }
                    break;
                #endregion
                #region length3
                case 3:
                    switch (c1) {
                        case 'R':
                            switch (c2) {
                                case 'A': return (c3 == 'X');
                                case 'B': return (c3 == 'X') || (c3 == 'P');
                                case 'C': return (c3 == 'X');
                                case 'D': return (c3 == 'X') || (c3 == 'I');
                                case 'S': return (c3 == 'I') || (c3 == 'P');
                                case '8': return (c3 == 'D') || (c3 == 'W') || (c3 == 'B');
                                case '9': return (c3 == 'D') || (c3 == 'W') || (c3 == 'B');
                                case '1': return (c3 == '0') || (c3 == '1') || (c3 == '2') || (c3 == '3') || (c3 == '4');
                            }
                            break;
                        case 'E':
                            switch (c2) {
                                case 'A': return (c3 == 'X');
                                case 'B': return (c3 == 'X') || (c3 == 'P');
                                case 'C': return (c3 == 'X');
                                case 'D': return (c3 == 'X') || (c3 == 'I');
                                case 'S': return (c3 == 'I') || (c3 == 'P');
                            }
                            break;
                        case 'B': return (c2 == 'P') && (c3 == 'L');
                        case 'S':
                            switch (c2) {
                                case 'P': return (c3 == 'L');
                                case 'I': return (c3 == 'L');
                            }
                            break;
                        case 'D': return (c2 == 'I') && (c3 == 'L');
                        case 'M':
                            if (c2 == 'M') {
                                switch (c3) {
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7': return true;
                                }
                            }
                            break;
                    }
                    break;
                #endregion
                #region length4
                case 4:
                    switch (c1) {
                        case 'R':
                            if (c2 == '1') {
                                switch (c3) {
                                    case '0': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                    case '1': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                    case '2': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                    case '3': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                    case '4': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                    case '5': return (c4 == 'D') || (c4 == 'W') || (c4 == 'B');
                                }
                            }
                            break;
                        case 'X':
                            if ((c2 == 'M') && (c3 == 'M')) {
                                return isNumber(c4);
                            }
                            break;
                        case 'Y':
                            if ((c2 == 'M') && (c3 == 'M')) {
                                return isNumber(c4);
                            }
                            break;
                        case 'Z':
                            if ((c2 == 'M') && (c3 == 'M')) {
                                return isNumber(c4);
                            }
                            break;
                    }
                    break;
                #endregion
                #region length5
                case 5:
                    switch (c1) {
                        case 'X':
                            if ((c2 == 'M') && (c3 == 'M') && (c4 == '1')) {
                                switch (c5) {
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5': return true;
                                }
                            }
                            break;
                        case 'Y':
                            if ((c2 == 'M') && (c3 == 'M') && (c4 == '1')) {
                                switch (c5) {
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5': return true;
                                }
                            }
                            break;
                        case 'Z':
                            if ((c2 == 'M') && (c3 == 'M')) {
                                switch (c4) {
                                    case '1': return isNumber(c5);
                                    case '2': return isNumber(c5);
                                    case '3': return ((c5 == '0') || (c5 == '1'));
                                }
                            }
                            break;
                    }
                    break;
                    #endregion
            }
            return false;
        }

        private static bool isNumber(char c) {
            switch (c) {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': return true;
                default: return false;
            }
        }

        public static bool isRegister(string keyword) {
            int length = keyword.Length;
            if ((length > 5) || (length < 2)) {
                return false;
            }
            bool b2 = isRegisterMethod2(keyword);
#           if _DEBUG
            if (b2 != isRegisterMethod1(keyword)) Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "INFO: isRegister; unequal responses"));
#           endif
            return b2;
        }





        public static bool isMmx(Rn rn) {
            switch (rn) {
                case Rn.MM0:
                case Rn.MM1:
                case Rn.MM2:
                case Rn.MM3:
                case Rn.MM4:
                case Rn.MM5:
                case Rn.MM6:
                case Rn.MM7: return true;
                default: return false;
            }
        }
        public static bool isSse(Rn rn) {
            switch (rn) {
                case Rn.XMM0:
                case Rn.XMM1:
                case Rn.XMM2:
                case Rn.XMM3:
                case Rn.XMM4:
                case Rn.XMM5:
                case Rn.XMM6:
                case Rn.XMM7:
                case Rn.XMM8:
                case Rn.XMM9:
                case Rn.XMM10:
                case Rn.XMM11:
                case Rn.XMM12:
                case Rn.XMM13:
                case Rn.XMM14:
                case Rn.XMM15: return true;
                default: return false;
            }
        }
        public static bool isAvx(Rn rn) {
            switch (rn) {
                case Rn.YMM0:
                case Rn.YMM1:
                case Rn.YMM2:
                case Rn.YMM3:
                case Rn.YMM4:
                case Rn.YMM5:
                case Rn.YMM6:
                case Rn.YMM7:
                case Rn.YMM8:
                case Rn.YMM9:
                case Rn.YMM10:
                case Rn.YMM11:
                case Rn.YMM12:
                case Rn.YMM13:
                case Rn.YMM14:
                case Rn.YMM15: return true;
                default: return false;
            }
        }
        public static bool isAvx512(Rn rn) {
            switch (rn) {
                case Rn.ZMM0:
                case Rn.ZMM1:
                case Rn.ZMM2:
                case Rn.ZMM3:
                case Rn.ZMM4:
                case Rn.ZMM5:
                case Rn.ZMM6:
                case Rn.ZMM7:
                case Rn.ZMM8:
                case Rn.ZMM9:
                case Rn.ZMM10:
                case Rn.ZMM11:
                case Rn.ZMM12:
                case Rn.ZMM13:
                case Rn.ZMM14:
                case Rn.ZMM15:
                case Rn.ZMM16:
                case Rn.ZMM17:
                case Rn.ZMM18:
                case Rn.ZMM19:
                case Rn.ZMM20:
                case Rn.ZMM21:
                case Rn.ZMM22:
                case Rn.ZMM23:
                case Rn.ZMM24:
                case Rn.ZMM25:
                case Rn.ZMM26:
                case Rn.ZMM27:
                case Rn.ZMM28:
                case Rn.ZMM29:
                case Rn.ZMM30:
                case Rn.ZMM31: return true;
                default: return false;
            }
        }

    }
}
