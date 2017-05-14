﻿// The MIT License (MIT)
//
// Copyright (c) 2017 Henk-Jan Lebbink
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AsmTools;
using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AsmSim
{
    public static class ToolsZ3
    {
        static readonly object _object = new object();

        #region Public Methods

        public static ulong GetRandomUlong(Random rand)
        {
            ulong i1, i2;
            lock (_object)
            {
                i1 = (ulong)rand.Next();
                i2 = (ulong)rand.Next();
            }
            return (i1 << 32) | i2;
        }

        public static int GetLineNumberFromLabel(string label, char lineNumberSeparator)
        {
            int beginPos = label.Length;
            for (int i = 0; i < label.Length; ++i)
            {
                if (label[i] == lineNumberSeparator)
                {
                    beginPos = i + 1;
                    break;
                }
            }
            int endPos = label.Length;

            //Console.WriteLine("INFO: ToolsZ3:getLineNumberFromLabel: label=" + label + "; beginPos="+ beginPos+"; endPos="+endPos);
            if (beginPos < endPos)
            {
                int result = -1;
                string substr = label.Substring(beginPos, endPos - beginPos);
                //Console.WriteLine("INFO: ToolsZ3:getLineNumberFromLabel: substr=" + substr + ".");
                if (Int32.TryParse(substr, out result))
                {
                    return result;
                }
            }
            return -1;
        }

        /// <summary>Cleans the provided line by removing multiple white spaces and cropping if the line is too long</summary>
        public static string Cleanup(string line, int maxNumberOfCharsOnLine = 150)
        {
            string cleanedString = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ").Trim();
            if (cleanedString.Length > maxNumberOfCharsOnLine)
            {
                return cleanedString.Substring(0, maxNumberOfCharsOnLine - 3) + "...";
            }
            else
            {
                return cleanedString;
            }
        }

        public static BoolExpr GetBit(BitVecExpr value, BitVecExpr pos, Context ctx)
        {
            return ctx.MkEq(GetBit_BV(value, pos, ctx), ctx.MkBV(1, 1));
        }
        public static BitVecExpr GetBit_BV(BitVecExpr value, BitVecExpr pos, Context ctx)
        {
            if (true)
            {
                return ctx.MkExtract(0, 0, ctx.MkBVLSHR(value, pos));
            }
            else
            {
                BitVecExpr mask = ctx.MkBVSHL(ctx.MkBV(1, value.SortSize), pos);
                ctx.MkBVRedOR(ctx.MkBVAND(value, mask));
            }
        }
        public static BoolExpr GetBit(BitVecExpr value, uint pos, BitVecNum one, Context ctx)
        {
            Debug.Assert(one.SortSize == 1);
            Debug.Assert(one.Int == 1);
            return ctx.MkEq(GetBit_BV(value, pos, ctx), one);
        }
        public static BitVecExpr GetBit_BV(BitVecExpr value, uint pos, Context ctx)
        {
            Debug.Assert(ctx != null, "Context ctx cannot be null");
            Debug.Assert(value != null, "BitVecExpr v cannot be null");
            return ctx.MkExtract(pos, pos, value);
        }

        public static (BitVecExpr value, BitVecExpr undef) MakeVecExpr(Tv[] tv5, Context ctx)
        {
            Debug.Assert(tv5.Length > 0);

            BitVecNum one = ctx.MkBV(1, 1);
            BitVecNum zero = ctx.MkBV(0, 1);
            Random random = new Random();

            BitVecExpr value = null;
            BitVecExpr undef = null;

            for (int i = 0; i < tv5.Length; ++i)
            {
                BitVecExpr next_value = null;
                BitVecExpr next_undef = null;
                switch (tv5[i])
                {
                    case Tv.UNDEFINED:
                        int rand_Int = random.Next();
                        next_value = ctx.MkBVConst("U" + rand_Int, 1);
                        next_undef = ctx.MkBVConst("U" + rand_Int, 1);
                        break;
                    case Tv.UNKNOWN:
                        next_value = ctx.MkBVConst("?" + random.Next(), 1);
                        next_undef = zero; // could also use one, 
                        break;
                    case Tv.ONE:
                        next_value = one;
                        next_undef = one;
                        break;
                    case Tv.ZERO:
                        next_value = zero;
                        next_undef = zero;
                        break;
                    case Tv.INCONSISTENT: throw new Exception();

                }
                value = (value == null) ? next_value : ctx.MkConcat(next_value, value);
                undef = (undef == null) ? next_undef : ctx.MkConcat(next_undef, undef);
            }
            value = value.Simplify() as BitVecExpr;
            undef = undef.Simplify() as BitVecExpr;

            return (value:value, undef:undef);
        }

        private static (bool valid, ulong value) IsSimpleAssignment(string name, BoolExpr e)
        {
            if (e.IsEq)
            {
                if (e.Args[0].IsConst)
                {
                    if (e.Args[0].ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        //Console.WriteLine("isSimpleAssignment: " + e + "; found name " + name+ "; type second argument ="+ e.Args[1].GetType());

                        if (e.Args[1].GetType().Equals(typeof(Microsoft.Z3.BitVecNum)))
                        {
                            //Console.WriteLine("isSimpleAssignment: e=" + e + "; second argument is numeral "+e.Args[1]);
                            BitVecNum value = e.Args[1] as BitVecNum;

                            return (valid: true, value: value.UInt64);
                        }
                    }
                }
            }
            return (valid: false, value: 0);
        }

        /// <summary>
        /// Consolidate constraints. Remove all redundancies.
        /// Eg. (= rax, rbx-4443) and (= rbx-4443, 2) is replaced by (= rax, 2)
        /// </summary>
        public static void Consolidate(bool undef, Solver solver, Solver solver_U, Context ctx)
        {
            if (true)
            {
                #region Doc
                /*
                    tacticName ackermannize_bv: A tactic for performing full Ackermannization on bv instances.
                    tacticName subpaving: tactic for testing subpaving module.
                    tacticName horn: apply tactic for horn clauses.
                    tacticName horn-simplify: simplify horn clauses.
                    tacticName nlsat: (try to) solve goal using a nonlinear arithmetic solver.
                    tacticName qfnra-nlsat: builtin strategy for solving QF_NRA problems using only nlsat.
                    tacticName nlqsat: apply a NL-QSAT solver.
                    tacticName qe-light: apply light-weight quantifier elimination.
                    tacticName qe-sat: check satisfiability of quantified formulas using quantifier elimination.
                    tacticName qe: apply quantifier elimination.
                    tacticName qsat: apply a QSAT solver.
                    tacticName qe2: apply a QSAT based quantifier elimination.
                    tacticName qe_rec: apply a QSAT based quantifier elimination recursively.
                    tacticName vsubst: checks satsifiability of quantifier-free non-linear constraints using virtual substitution.
                    tacticName sat: (try to) solve goal using a SAT solver.
                    tacticName sat-preprocess: Apply SAT solver preprocessing procedures (bounded resolution, Boolean constant propagation, 2-SAT, subsumption, subsumption resolution).
                    tacticName ctx-solver-simplify: apply solver-based contextual simplification rules.
                    tacticName smt: apply a SAT based SMT solver.
                    tacticName unit-subsume-simplify: unit subsumption simplification.
                    tacticName aig: simplify Boolean structure using AIGs.
                    tacticName add-bounds: add bounds to unbounded variables (under approximation).
                    tacticName card2bv: convert pseudo-boolean constraints to bit-vectors.
                    tacticName degree-shift: try to reduce degree of polynomials (remark: :mul2power simplification is automatically applied).
                    tacticName diff-neq: specialized solver for integer arithmetic problems that contain only atoms of the form (<= k x) (<= x k) and (not (= (- x y) k)), where x and y are constants and k is a numeral, and all constants are bounded.
                    tacticName elim01: eliminate 0-1 integer variables, replace them by Booleans.
                    tacticName eq2bv: convert integer variables used as finite domain elements to bit-vectors.
                    tacticName factor: polynomial factorization.
                    tacticName fix-dl-var: if goal is in the difference logic fragment, then fix the variable with the most number of occurrences at 0.
                    tacticName fm: eliminate variables using fourier-motzkin elimination.
                    tacticName lia2card: introduce cardinality constraints from 0-1 integer.
                    tacticName lia2pb: convert bounded integer variables into a sequence of 0-1 variables.
                    tacticName nla2bv: convert a nonlinear arithmetic problem into a bit-vector problem, in most cases the resultant goal is an under approximation and is useul for finding models.
                    tacticName normalize-bounds: replace a variable x with lower bound k <= x with x' = x - k.
                    tacticName pb2bv: convert pseudo-boolean constraints to bit-vectors.
                    tacticName propagate-ineqs: propagate ineqs/bounds, remove subsumed inequalities.
                    tacticName purify-arith: eliminate unnecessary operators: -, /, div, mod, rem, is-int, to-int, ^, root-objects.
                    tacticName recover-01: recover 0-1 variables hidden as Boolean variables.
                    tacticName bit-blast: reduce bit-vector expressions into SAT.
                    tacticName bv1-blast: reduce bit-vector expressions into bit-vectors of size 1 (notes: only equality, extract and concat are supported).
                    tacticName bv_bound_chk: attempts to detect inconsistencies of bounds on bv expressions.
                    tacticName propagate-bv-bounds: propagate bit-vector bounds by simplifying implied or contradictory bounds.
                    tacticName reduce-bv-size: try to reduce bit-vector sizes using inequalities.
                    tacticName bvarray2uf: Rewrite bit-vector arrays into bit-vector (uninterpreted) functions.
                    tacticName dt2bv: eliminate finite domain data-types. Replace by bit-vectors.
                    tacticName elim-small-bv: eliminate small, quantified bit-vectors by expansion.
                    tacticName max-bv-sharing: use heuristics to maximize the sharing of bit-vector expressions such as adders and multipliers.
                    tacticName blast-term-ite: blast term if-then-else by hoisting them.
                    tacticName cofactor-term-ite: eliminate term if-the-else using cofactors.
                    tacticName collect-statistics: Collects various statistics.
                    tacticName ctx-simplify: apply contextual simplification rules.
                    tacticName der: destructive equality resolution.
                    tacticName distribute-forall: distribute forall over conjunctions.
                    tacticName elim-term-ite: eliminate term if-then-else by adding fresh auxiliary declarations.
                    tacticName elim-uncnstr: eliminate application containing unconstrained variables.
                    tacticName snf: put goal in skolem normal form.
                    tacticName nnf: put goal in negation normal form.
                    tacticName occf: put goal in one constraint per clause normal form (notes: fails if proof generation is enabled; only clauses are considered).
                    tacticName pb-preprocess: pre-process pseudo-Boolean constraints a la Davis Putnam.
                    tacticName propagate-values: propagate constants.
                    tacticName reduce-args: reduce the number of arguments of function applications, when for all occurrences of a function f the i-th is a value.
                    tacticName simplify: apply simplification rules.
                    tacticName elim-and: convert (and a b) into (not (or (not a) (not b))).
                    tacticName solve-eqs: eliminate variables by solving equations.
                    tacticName split-clause: split a clause in many subgoals.
                    tacticName symmetry-reduce: apply symmetry reduction.
                    tacticName tseitin-cnf: convert goal into CNF using tseitin-like encoding (note: quantifiers are ignored).
                    tacticName tseitin-cnf-core: convert goal into CNF using tseitin-like encoding (note: quantifiers are ignored). This tactic does not apply required simplifications to the input goal like the tseitin-cnf tactic.
                    tacticName fpa2bv: convert floating point numbers to bit-vectors.
                    tacticName qffp: (try to) solve goal using the tactic for QF_FP.
                    tacticName qffpbv: (try to) solve goal using the tactic for QF_FPBV (floats+bit-vectors).
                    tacticName nl-purify: Decompose goal into pure NL-sat formula and formula over other theories.
                    tacticName default: default strategy used when no logic is specified.
                    tacticName qfbv-sls: (try to) solve using stochastic local search for QF_BV.
                    tacticName nra: builtin strategy for solving NRA problems.
                    tacticName qfaufbv: builtin strategy for solving QF_AUFBV problems.
                    tacticName qfauflia: builtin strategy for solving QF_AUFLIA problems.
                    tacticName qfbv: builtin strategy for solving QF_BV problems.
                    tacticName qfidl: builtin strategy for solving QF_IDL problems.
                    tacticName qflia: builtin strategy for solving QF_LIA problems.
                    tacticName qflra: builtin strategy for solving QF_LRA problems.
                    tacticName qfnia: builtin strategy for solving QF_NIA problems.
                    tacticName qfnra: builtin strategy for solving QF_NRA problems.
                    tacticName qfuf: builtin strategy for solving QF_UF problems.
                    tacticName qfufbv: builtin strategy for solving QF_UFBV problems.
                    tacticName qfufbv_ackr: A tactic for solving QF_UFBV based on Ackermannization.
                    tacticName qfufnra: builtin strategy for solving QF_UNFRA problems.
                    tacticName ufnia: builtin strategy for solving UFNIA problems.
                    tacticName uflra: builtin strategy for solving UFLRA problems.
                    tacticName auflia: builtin strategy for solving AUFLIA problems.
                    tacticName auflira: builtin strategy for solving AUFLIRA problems.
                    tacticName aufnira: builtin strategy for solving AUFNIRA problems.
                    tacticName lra: builtin strategy for solving LRA problems.
                    tacticName lia: builtin strategy for solving LIA problems.
                    tacticName lira: builtin strategy for solving LIRA problems.
                    tacticName skip: do nothing tactic.
                    tacticName fail: always fail tactic.
                    tacticName fail-if-undecided: fail if goal is undecided.
                    tacticName macro-finder: Identifies and applies macros.
                    tacticName quasi-macros: Identifies and applies quasi-macros.
                    tacticName ufbv-rewriter: Applies UFBV-specific rewriting rules, mainly demodulation.
                    tacticName bv: builtin strategy for solving BV problems (with quantifiers).
                    tacticName ufbv: builtin strategy for solving UFBV problems (with quantifiers).
                */
                #endregion
                {
                    Tactic taY = ctx.MkTactic("default"); // does not work: all constraitns are lost
                    Tactic taX = ctx.MkTactic("skip");
                    Tactic ta0 = ctx.MkTactic("ctx-solver-simplify"); //VERY SLOW
                    Tactic ta1 = ctx.MkTactic("simplify"); // some minor rewrites
                    Tactic ta2 = ctx.MkTactic("ctx-simplify"); // no differences compared with SKIP
                    Tactic ta3 = ctx.MkTactic("qfbv"); // does not work: all constraitns are lost
                    Tactic ta3b = ctx.MkTactic("qfbv-sls");// does not work: all constraitns are lost
                    Tactic ta4 = ctx.MkTactic("solve-eqs");// does not work: all constraitns are lost
                    Tactic ta5 = ctx.MkTactic("propagate-values"); // no differences compared with SKIP
                    Tactic ta6 = ctx.MkTactic("sat"); // make much more constrains such that Flatten takes VERY LONG
                    Tactic ta6b = ctx.MkTactic("sat-preprocess"); // does not work: some constrains are lost
                    Tactic ta7 = ctx.MkTactic("smt"); // does not work: all constraitns are lost


                    Tactic tactic = ctx.AndThen(ta2, ta1);
                    if (!undef) {
                        Goal goal1 = ctx.MkGoal();
                        goal1.Assert(solver.Assertions);
                        ApplyResult ar = tactic.Apply(goal1);
                        solver.Reset();
                        solver.Assert(ar.Subgoals[0].Formulas);
                    } else
                    {
                        Goal goal1 = ctx.MkGoal();
                        goal1.Assert(solver_U.Assertions);
                        ApplyResult ar = tactic.Apply(goal1);
                        solver_U.Reset();
                        solver_U.Assert(ar.Subgoals[0].Formulas);
                    }
                }
            }
        }

        /// <summary>Returns true if the provided valueExpr and undef yield the same tv5 array as the provided valueTv </summary>
        public static bool Equals(BitVecExpr valueExpr, BitVecExpr undef, Tv[] valueTv, int nBits, Solver solver, Solver solver_U, Context ctx)
        {
            BitVecNum bv1_1bit = ctx.MkBV(1, 1);
            for (uint bit = 0; bit < nBits; ++bit)
            {
                BoolExpr b = ToolsZ3.GetBit(valueExpr, bit, bv1_1bit, ctx);
                BoolExpr b_undef = ToolsZ3.GetBit(undef, bit, bv1_1bit, ctx);
                // this can be done faster
                Tv tv = ToolsZ3.GetTv(b, b_undef, solver, solver_U, ctx);
                if (tv != valueTv[bit]) return false;
            }
            return true;
        }

        public static bool Equals(BoolExpr valueExpr, BoolExpr undef, Tv valueTv, Solver solver, Solver solver_U, Context ctx)
        {
            // this can be done faster
            Tv tv = ToolsZ3.GetTv(valueExpr, undef, solver, solver_U, ctx);
            return (tv == Tv.ONE);
        }

        private static Flags CollectFlags_UNUSED(Expr e)
        {
            Flags flags = 0;
            CollectFlags_UNUSED(e, ref flags);
            return flags;
        }

        private static void CollectFlags_UNUSED(Expr e, ref Flags flags)
        {
            if (e.IsConst)
            {
                flags |= FlagTools.Parse(e.ToString().Substring(0, 2));
            }
            else
            {
                foreach (Expr e2 in e.Args)
                {
                    ToolsZ3.CollectFlags_UNUSED(e2, ref flags);
                }
            }
        }

        /// <summary>add the contants from Expression e to the provided set</summary>
        private static void CollectConstants_UNUSED(Expr e, ref ISet<Expr> set)
        {
            if (e.IsConst)
            {
                set.Add(e);
            }
            else
            {
                foreach (Expr e2 in e.Args)
                {
                    ToolsZ3.CollectConstants_UNUSED(e2, ref set);
                }
            }
        }

        public static string ToString(Expr e)
        {
            if (false)
            {
                return e.ToString();
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Replace(e.ToString(), @"\s+", " ");
            }
        }

        #region Print Methods

        public static string ToStringBin(Tv[] a)
        {
            StringBuilder sb = new StringBuilder("0b");
            if (a == null)
            {
                sb.Append("null");
            }
            else
            {
                int nBits = a.Length;
                for (int i = (nBits - 1); i >= 0; --i)
                {
                    sb.Append(ToolsZ3.ToStringBin(a[i]));
                    if ((i > 0) && (i != nBits - 1) && (i % 8 == 0)) sb.Append('.');
                }
            }
            return sb.ToString();
        }

        public static string ToStringHex(Tv[] a)
        {
            StringBuilder sb = new StringBuilder("0x");
            int nChars = a.Length >> 2;
            for (int j = (nChars - 1); j >= 0; --j)
            {
                int offset = (j << 2);
                sb.Append(ToolsZ3.BitToCharHex(a[offset], a[offset + 1], a[offset + 2], a[offset + 3]));

                if ((j > 0) && ((j % 8) == 0)) sb.Append('.');
            }
            return sb.ToString();
        }

        public static char ToStringBin(Tv tv)
        {
            switch (tv)
            {
                case Tv.UNDEFINED: return 'U';
                case Tv.UNKNOWN: return '?';
                case Tv.ONE: return '1';
                case Tv.ZERO: return '0';
                case Tv.INCONSISTENT: return 'X';
                case Tv.UNDETERMINED: return '-';
                default: return 'Y';
            }
        }

        public static char BitToCharHex(Tv b0, Tv b1, Tv b2, Tv b3)
        {
            if ((b3 == Tv.UNDETERMINED) || (b2 == Tv.UNDETERMINED) || (b1 == Tv.UNDETERMINED) || (b0 == Tv.UNDETERMINED)) return '-';
            if ((b3 == Tv.UNDEFINED) || (b2 == Tv.UNDEFINED) || (b1 == Tv.UNDEFINED) || (b0 == Tv.UNDEFINED)) return 'U';
            if ((b3 == Tv.UNKNOWN) || (b2 == Tv.UNKNOWN) || (b1 == Tv.UNKNOWN) || (b0 == Tv.UNKNOWN)) return '?';
            if ((b3 == Tv.INCONSISTENT) || (b2 == Tv.INCONSISTENT) || (b1 == Tv.INCONSISTENT) || (b0 == Tv.INCONSISTENT)) return 'X';

            if ((b3 == Tv.ZERO) && (b2 == Tv.ZERO) && (b1 == Tv.ZERO) && (b0 == Tv.ZERO)) return '0';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ZERO) && (b1 == Tv.ZERO) && (b0 == Tv.ONE)) return '1';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ZERO) && (b1 == Tv.ONE) && (b0 == Tv.ZERO)) return '2';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ZERO) && (b1 == Tv.ONE) && (b0 == Tv.ONE)) return '3';

            if ((b3 == Tv.ZERO) && (b2 == Tv.ONE) && (b1 == Tv.ZERO) && (b0 == Tv.ZERO)) return '4';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ONE) && (b1 == Tv.ZERO) && (b0 == Tv.ONE)) return '5';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ONE) && (b1 == Tv.ONE) && (b0 == Tv.ZERO)) return '6';
            if ((b3 == Tv.ZERO) && (b2 == Tv.ONE) && (b1 == Tv.ONE) && (b0 == Tv.ONE)) return '7';

            if ((b3 == Tv.ONE) && (b2 == Tv.ZERO) && (b1 == Tv.ZERO) && (b0 == Tv.ZERO)) return '8';
            if ((b3 == Tv.ONE) && (b2 == Tv.ZERO) && (b1 == Tv.ZERO) && (b0 == Tv.ONE)) return '9';
            if ((b3 == Tv.ONE) && (b2 == Tv.ZERO) && (b1 == Tv.ONE) && (b0 == Tv.ZERO)) return 'A';
            if ((b3 == Tv.ONE) && (b2 == Tv.ZERO) && (b1 == Tv.ONE) && (b0 == Tv.ONE)) return 'B';

            if ((b3 == Tv.ONE) && (b2 == Tv.ONE) && (b1 == Tv.ZERO) && (b0 == Tv.ZERO)) return 'C';
            if ((b3 == Tv.ONE) && (b2 == Tv.ONE) && (b1 == Tv.ZERO) && (b0 == Tv.ONE)) return 'D';
            if ((b3 == Tv.ONE) && (b2 == Tv.ONE) && (b1 == Tv.ONE) && (b0 == Tv.ZERO)) return 'E';
            if ((b3 == Tv.ONE) && (b2 == Tv.ONE) && (b1 == Tv.ONE) && (b0 == Tv.ONE)) return 'F';

            // unreachable
            return 'Y';
        }

        #endregion Print Methods

        #region Conversion 
        public static ulong? GetUlong(BitVecExpr value, uint nBits, Solver solver, Context ctx)
        {
            if (value.IsBVNumeral)
            {
                return ((BitVecNum)value).UInt64;
            }

            Tv[] results = new Tv[nBits];
            BitVecNum ONE = ctx.MkBV(1, 1);

            for (uint bit = 0; bit < nBits; ++bit)
            {
                BoolExpr b = ToolsZ3.GetBit(value, bit, ONE, ctx);
                switch (ToolsZ3.GetTv(b, solver, ctx))
                {
                    case Tv.ONE:
                        results[bit] = Tv.ONE;
                        break;
                    case Tv.ZERO:
                        results[bit] = Tv.ZERO;
                        break;
                    default:
                        return null;
                }
            }
            return ToolsZ3.GetUlong(results);
        }
        public static ulong? GetUlong(Tv[] array)
        {
            ulong result = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                switch (array[i])
                {
                    case Tv.ONE:
                        result |= (1UL << i);
                        break;
                    case Tv.ZERO:
                        break;
                    case Tv.UNKNOWN:
                    case Tv.INCONSISTENT:
                    case Tv.UNDETERMINED:
                    default: return null;
                }
            }
            return result;
        }

        public static Tv[] GetTvArray(ulong value, int nBits)
        {
            Tv[] result = new Tv[nBits];
            for (int i = 0; i < nBits; ++i)
            {
                result[i] = (((value >> i) & 1L) == 1L) ? Tv.ONE : Tv.ZERO;
            }
            return result;
        }
        public static Tv[] GetTvArray(string value)
        {
            char[] charArray = value.Replace(".", "").Replace("_", "").ToCharArray();
            Array.Reverse(charArray);

            int nBits = charArray.Length;
            Tv[] result = new Tv[nBits];
            for (int i = 0; i < nBits; ++i)
            {
                switch (charArray[i])
                {
                    case 'U':
                        result[i] = Tv.UNDEFINED;
                        break;
                    case '?':
                        result[i] = Tv.UNKNOWN;
                        break;
                    case '0':
                        result[i] = Tv.ZERO;
                        break;
                    case '1':
                        result[i] = Tv.ONE;
                        break;
                    case 'X':
                        result[i] = Tv.INCONSISTENT;
                        break;
                    case '-':
                        result[i] = Tv.UNDETERMINED;
                        break;
                    default: throw new Exception();
                }
            }
            return result;
        }
        public static Tv[] GetTvArray(BitVecExpr value, int nBits, Solver solver, Solver solver_U, Context ctx)
        {
            return GetTvArray(value, value, nBits, solver, solver_U, ctx);
        }
        public static Tv[] GetTvArray(BitVecExpr value, BitVecExpr undef, int nBits, Solver solver, Solver solver_U, Context ctx)
        {
            Tv[] results = new Tv[nBits];
            if (value == null)
            {
                Console.WriteLine("WARNING: ToolsZ3:GetTv5Array: value is null, assuming UNKNOWN");
                return results;
            }
            BitVecNum bv1_1bit = ctx.MkBV(1, 1);
            for (uint bit = 0; bit < nBits; ++bit)
            {
                BoolExpr b = ToolsZ3.GetBit(value, bit, bv1_1bit, ctx);
                BoolExpr b_undef = ToolsZ3.GetBit(undef, bit, bv1_1bit, ctx);
                results[bit] = ToolsZ3.GetTv(b, b_undef, solver, solver_U, ctx);
            }
            return results;
        }
        public static Tv[] GetTvArray(BitVecExpr value, int nBits, Solver solver, Context ctx)
        {
            Tv[] results = new Tv[nBits];
            if (value == null)
            {
                Console.WriteLine("WARNING: ToolsZ3:GetTv5Array: value is null, assuming UNKNOWN");
                return results;
            }
            BitVecNum bv1_1bit = ctx.MkBV(1, 1);
            for (uint bit = 0; bit < nBits; ++bit)
            {
                BoolExpr b = ToolsZ3.GetBit(value, bit, bv1_1bit, ctx);
                results[bit] = ToolsZ3.GetTv(b, solver, ctx);
            }
            return results;
        }

        public static Tv GetTv(BoolExpr value, BoolExpr undef, Solver solver, Solver solver_U, Context ctx)
        {
            bool tvTrue;
            {
                Status status = solver.Check(value);
                if (status == Status.UNKNOWN)
                {
                    Console.WriteLine("ToolsZ3:getTv5: A: value=" + value + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                    return Tv.UNDETERMINED;
                }
                tvTrue = (status == Status.SATISFIABLE);
            }

            //if (!tvTrue) return Tv5.ZERO;

            bool tvFalse;
            {
                Status status = solver.Check(ctx.MkNot(value));
                if (status == Status.UNKNOWN)
                {
                    Console.WriteLine("ToolsZ3:getTv5: B: value=" + ctx.MkNot(value) + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                    return Tv.UNDETERMINED;
                }
                tvFalse = (status == Status.SATISFIABLE);
            }
            // if it consistent to assert that the provided bit is true, 
            // and seperately that it is consistent to be false, the model in the solver 
            // is indifferent about the truth-value of bit.

            if (!tvTrue && !tvFalse) return Tv.INCONSISTENT; // TODO: if inconsistent does not occur and !tvTrue is observed we can directly return Tv5.ZERO 
            if (!tvTrue && tvFalse) return Tv.ZERO;
            if (tvTrue && !tvFalse) return Tv.ONE;
            if (tvTrue && tvFalse) // truth value of bit cannot be determined is not known: it is either UNKNOWN or UNDEFINED
            {
                if (solver_U == null) return Tv.UNKNOWN;
                bool tvFalseU;
                {
                    Status status = solver_U.Check(ctx.MkNot(undef));
                    if (status == Status.UNKNOWN)
                    {
                        Console.WriteLine("ToolsZ3:getTv5: C: undef=" + ctx.MkNot(undef) + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                        return Tv.UNDETERMINED;
                    } 
                    tvFalseU = (status == Status.SATISFIABLE);
                }
                // if (!tvFalseU) return Tv5.UNKNOWN;

                bool tvTrueU;
                {
                    Status status = solver_U.Check(undef);
                    if (status == Status.UNKNOWN)
                    {
                        Console.WriteLine("ToolsZ3:getTv5: D: undef=" + undef + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                        return Tv.UNDETERMINED;
                    }
                    tvTrueU = (status == Status.SATISFIABLE);
                }
                return (tvTrueU && tvFalseU) ? Tv.UNDEFINED : Tv.UNKNOWN;
            }

            // unreachable
            throw new Exception();
        }
        public static Tv GetTv(BoolExpr value, Solver solver, Context ctx)
        {
            bool tvTrue;
            {
                solver.Push();
                solver.Assert(value);
                //Status status = solver.Check(value);
                Status status = solver.Check();
                if (status == Status.UNKNOWN) {
                    Console.WriteLine("ToolsZ3:getTv5: A: value=" + value + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                    return Tv.UNDETERMINED;
                }
                tvTrue = (status == Status.SATISFIABLE);
                solver.Pop();
            }

            //if (!tvTrue) return Tv5.ZERO;

            bool tvFalse;
            {
                solver.Push();
                solver.Assert(ctx.MkNot(value));
                Status status = solver.Check();
                if (status == Status.UNKNOWN) {
                    Console.WriteLine("ToolsZ3:getTv5: B: value=" + ctx.MkNot(value) + " yields UNKNOWN solver status. Reason: " + solver.ReasonUnknown);
                    return Tv.UNDETERMINED;
                }
                tvFalse = (status == Status.SATISFIABLE);
                solver.Pop();
            }
            // if it consistent to assert that the provided bit is true, 
            // and seperately that it is consistent to be false, the model in the solver 
            // is indifferent about the truth-value of bit.

            if (!tvTrue && !tvFalse) return Tv.INCONSISTENT; // TODO: if inconsistent does not occur and !tvTrue is observed we can directly return Tv5.ZERO 
            if (!tvTrue && tvFalse) return Tv.ZERO;
            if (tvTrue && !tvFalse) return Tv.ONE;
            if (tvTrue && tvFalse) return Tv.UNKNOWN;

            // unreachable
            throw new Exception();
        }

        #endregion

        #endregion Public Methods

        private static bool Contains_UNUSED(Expr e, string element)
        {
            if (e.IsConst)
            {
                if (e.ToString().Equals(element))
                {
                    return true;
                }
            }
            else
            {
                foreach (Expr e2 in e.Args)
                {
                    if (Contains_UNUSED(e2, element))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Expr UpdateConstName(Expr expr, string postfix, Context ctx)
        {
            var tup = GetConstants(expr);

            foreach (Symbol s in tup.Item1)
            {
                expr = expr.Substitute(ctx.MkBoolConst(s), ctx.MkBoolConst(s+postfix));
                //Console.WriteLine("UpdateConstName: s=" + s + "; expr=" + expr);
            }
            foreach (Symbol s in tup.Item2)
            {
                expr = expr.Substitute(ctx.MkBVConst(s, 64), ctx.MkBVConst(s + postfix, 64));
                //Console.WriteLine("UpdateConstName: s=" + s + "; expr=" + expr);
            }
            return expr;
        }

        private static (IList<Symbol>, IList<Symbol>) GetConstants(Expr expr)
        {
            IList<Symbol> boolResults = new List<Symbol>();
            IList<Symbol> bvResults = new List<Symbol>();
            ToolsZ3.GetConstants(expr, ref boolResults, ref bvResults);
            return (boolResults, bvResults);
        }

        /// <summary> check whethe provided array of truth-values only contains a single value, return this single value</summary>
        public static (bool hasOneValue, Tv value) HasOneValue(Tv[] array)
        {
            bool unknown = true;
            bool zero = true;
            bool one = true;
            bool inconsitent = true;
            bool undefined = true;

            for (int i = 0; i < array.Length; ++i)
            {
                switch (array[i])
                {
                    case Tv.UNKNOWN: zero = one = inconsitent = undefined = false; break;
                    case Tv.ZERO: unknown = one = inconsitent = undefined = false; break;
                    case Tv.ONE: unknown = zero = inconsitent = undefined = false; break;
                    case Tv.INCONSISTENT: unknown = zero = one = undefined = false; break;
                    case Tv.UNDEFINED: unknown = zero = one = inconsitent = false; break;
                }
            }
            if (unknown) return (hasOneValue: true, value: Tv.UNKNOWN);
            if (zero) return (hasOneValue: true, value: Tv.ZERO);
            if (one) return (hasOneValue: true, value: Tv.ONE);
            if (inconsitent) return (hasOneValue: true, value: Tv.INCONSISTENT);
            if (undefined) return (hasOneValue: true, value: Tv.UNDEFINED);

            return (hasOneValue: false, value: Tv.UNKNOWN);
        }

        private static void GetConstants(Expr expr, ref IList<Symbol> boolResults, ref IList<Symbol> bvResults)
        {
            if (expr.IsConst)
            {
                if (expr.IsBool)
                {
                    boolResults.Add(expr.FuncDecl.Name);
                } else
                {
                    bvResults.Add(expr.FuncDecl.Name);
                }
            }
            else
            {
                foreach (Expr expr2 in expr.Args)
                {
                    GetConstants(expr2, ref boolResults, ref bvResults);
                }
            }
        }
    }
}