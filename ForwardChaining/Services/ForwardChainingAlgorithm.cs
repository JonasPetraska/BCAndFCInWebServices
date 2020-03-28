using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForwardChaining.Services
{
    public class ForwardChainingAlgorithm
    {
        private RequestModel _model;
        private ResponseModel _responseModel;
        private List<char> _derivedFacts = new List<char>();
        private List<Rule> _usedRules = new List<Rule>();
        private List<Rule> _productions = new List<Rule>();
        private List<char> _goals = new List<char>();
        //private Queue<string> _root = new Queue<string>();
        private List<char> doesntHave = new List<char>();
        public ForwardChainingAlgorithm(RequestModel model)
        {
            _model = model;
            _responseModel = new ResponseModel(_model);
        }

        //Executes algorithm
        public ResponseModel Execute()
        {
            bool state = FC();

            if (state)
            {
                foreach (var i in _productions)
                    _responseModel.Production.Add(i);
            }

            _responseModel.Result = state ? Common.Enums.FCBCResultEnum.Success : Common.Enums.FCBCResultEnum.NoSuccess;
            return _responseModel;
        }

        private bool FC()
        {
            bool halt;
            int index = 0;
            int iteration = 0;
            var GDB = new List<char>(_responseModel.Facts);
            var goal = _responseModel.Goal;
            var rules = new List<Rule>(_responseModel.Rules);

            while (true)
            {
                if (GDB.Contains(goal))
                {
                    return true;
                }
                halt = true;
                //Trace += ++ruleNumbering + " iteracija \n\n";
                iteration++;
                index = 0;
                foreach (Rule r in rules)
                {
                    index++;
                    if (GDB.Contains(goal))
                    {
                        return true;
                    }
                    else
                    {
                        if (r.Flag2 == true)
                        {
                            _responseModel.Trace.Add(new TraceElement(iteration, Common.Enums.TraceElementTypeEnum.RuleNotApplicableFlag2Raised, r));
                            //Trace += "   " + index + ") R" + r.RuleNo + " (" + string.Join(", ", r.RightSide.ToList()) + " -> " + r.LeftSide
                            //                        + ") netaikoma, nes pakelta flag2.\n";
                        }
                        else
                        {
                            if (r.Flag1 == true)
                            {
                                _responseModel.Trace.Add(new TraceElement(iteration, Common.Enums.TraceElementTypeEnum.RuleNotApplicableFlag1Raised, r));
                                //Trace += "   " + index + ") R" + r.RuleNo + " (" + string.Join(", ", r.RightSide.ToList()) + " -> " + r.LeftSide
                                //                        + ") netaikoma, nes pakelta flag1.\n";
                            }
                            else
                            {
                                if (GDB.Contains(r.RightSide))
                                {
                                    r.Flag2 = true;
                                    _responseModel.Trace.Add(new TraceElement(iteration, Common.Enums.TraceElementTypeEnum.RuleNotApplicableResultInFactsRaiseFlag2, r));
                                    //Trace += "   " + index + ") R" + r.RuleNo + " (" + string.Join(", ", r.RightSide.ToList()) + " -> " + r.LeftSide
                                    //                    + ") netaikoma, nes rezultatas yra faktuose. Keliamas flag2.\n";
                                }
                                else
                                {
                                    if (IsInRuleLeftSide(r.LeftSide, GDB))
                                    {
                                        _usedRules.Add(r);
                                        halt = !halt;
                                        GDB.Add(r.RightSide);
                                        r.Flag1 = true;

                                        _responseModel.Trace.Add(new TraceElement(iteration, Common.Enums.TraceElementTypeEnum.RuleApplicableRaiseFlag1, r, GDB));

                                        //Trace += "   " + index + ") R" + r.RuleNo + " (" + string.Join(", ", r.RightSide.ToList()) + " -> " + r.LeftSide
                                        //                + ") taikoma, pakeliamas flag1. Faktai: {" + string.Join(", ", GDB.ToList()) + "}.\n\n";
                                        _productions.Add(r);
                                        break;
                                    }
                                    else
                                    {

                                        foreach (char c in r.LeftSide)
                                        {
                                            if (!GDB.Contains(c))
                                            {
                                                doesntHave.Add(c);
                                            }
                                        }
                                        //Trace += "   " + index + ") R" + r.RuleNo + " (" + string.Join(", ", r.RightSide.ToList()) + " -> " + r.LeftSide
                                        //                + ") netaikoma, nes truksta " + string.Join(", ", doesntHave.ToList()) + ".\n";

                                        _responseModel.Trace.Add(new TraceElement(iteration, Common.Enums.TraceElementTypeEnum.RuleNotApplicableMissingFacts, r, doesntHave));
                                        doesntHave = new List<char>();
                                    }
                                }
                            }
                        }
                    }
                }

                if (halt)
                {
                    //Trace += "\n";
                    return false;
                }
            }
        }

        private bool IsInUsedRules(Rule r)
        {
            if (_usedRules.Contains(r))
                return true;
            else
                return false;
        }

        private bool IsInRuleLeftSide(List<char> facts, List<char> GDB)
        {
            int cnt = 0;

            foreach (char c in facts)
            {
                if (GDB.Contains(c))
                {
                    cnt++;
                }
            }

            if (cnt == facts.Count)
                return true;
            else
                return false;
        }


    }
}
