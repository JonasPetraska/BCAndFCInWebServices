using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Enums
{
    public enum TraceElementTypeEnum
    {
        Fact,
        EarlierDerivedFact,
        NewFact,
        NewGoal,
        //FC
        RuleNotApplicableFlag2Raised,
        RuleNotApplicableFlag1Raised,
        RuleNotApplicableResultInFactsRaiseFlag2,
        RuleNotApplicableMissingFacts,
        RuleApplicableRaiseFlag1
    }
}
