using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackwardChaining.Services
{
    public class BackwardChainingAlgorithm
    {
        private RequestModel _model;
        private ResponseModel _responseModel;
        private List<char> _derivedFacts = new List<char>();
        private List<Rule> _usedRules = new List<Rule>();
        private List<Rule> _productions = new List<Rule>();
        private List<char> _goals = new List<char>();

        public BackwardChainingAlgorithm(RequestModel model)
        {
            _model = model;
            _responseModel = new ResponseModel(_model);
        }

        //Executes algorithm
        public ResponseModel Execute()
        {
            bool state = BC(_model.Goal, null);

            if (state)
            {
                foreach (var i in _productions)
                    _responseModel.Production.Add(i);
            }

            _responseModel.Result = state ? Common.Enums.FCBCResultEnum.Success : Common.Enums.FCBCResultEnum.NoSuccess;
            return _responseModel;
        }

        private bool BC(char goal, Rule usedRule)
        {
            if (!_goals.Contains(goal))
            {
                if (_responseModel.Facts.Contains(goal))
                {
                    if(usedRule != null)
                        _responseModel.Trace.Add(new TraceElement(usedRule, usedRule.LeftSide, usedRule.RightSide));

                    _responseModel.Trace.Add(new TraceElement(goal, goal));
                    return true;
                }
                else if (_derivedFacts.Contains(goal))
                {
                    if (usedRule != null)
                        _responseModel.Trace.Add(new TraceElement(usedRule, usedRule.LeftSide, usedRule.RightSide));

                    _responseModel.Trace.Add(new TraceElement(goal, goal, true));
                    return true;
                }
                else
                {
                    _goals.Add(goal);
                    if(usedRule != null)
                        _responseModel.Trace.Add(new TraceElement(usedRule, usedRule.LeftSide, usedRule.RightSide));

                    foreach (Rule rule in _responseModel.Rules)
                    {
                        if (rule.RightSide.Equals(goal))
                        {
                            bool usable = true;
                            List<char> temp_derived_facts = new List<char>();
                            List<Rule> temp_productions = new List<Rule>();
                            temp_derived_facts.AddRange(_derivedFacts);
                            temp_productions.AddRange(_productions);
                            foreach (char fact in rule.LeftSide)
                            {
                                if (!BC(fact, rule))
                                {
                                    usable = false;
                                    _derivedFacts = temp_derived_facts;
                                    _productions = temp_productions;
                                    break;
                                }
                            }

                            if (usable)
                            {
                                _derivedFacts.Add(rule.RightSide);
                                _productions.Add(rule);
                                _goals.Remove(goal);

                                _responseModel.Trace.Add(new TraceElement(rule.RightSide, _model.Facts.Union(_derivedFacts).ToList(), goal));
                                return true;
                            }
                        }
                    }

                    _goals.Remove(goal);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


    }
}
