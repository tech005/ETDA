using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static BotCore.Types.DebugLogger;

namespace BotCore.States
{
    [StateAttribute(Author: "ETDA", Desc: "Will cast fas spells on monsters within the specified radius", DefaultPriority: 55)]
    public class FasMonsters : GameState
    {
        public List<MapObject> Targets = new List<MapObject>();

        private string m_fasSpell = "mor fas nadur"; //by default we will assume we have mor fas nadur
        [Description("What Fas spell will we use?"), Category("Fas Spell Used")]
        [TypeConverter(typeof(FasSpellConverter))]
        public string UsingFasSpell
        {
            get { return m_fasSpell; }
            set { m_fasSpell = value; }
        }

        private bool m_HavePathRequired;
        [Description("Cast Fas Only if we have a path to the target."), Category("Fas Conditions")]
        public bool DontFasInvalidPath
        {
            get { return m_HavePathRequired; }
            set { m_HavePathRequired = value; }
        }

        private int m_CastingDistance = 9;
        [Description("Cast Fas Only if target is within X tiles"), Category("Casting Conditions")]
        public int CastingDistance
        {
            get { return m_CastingDistance; }
            set { m_CastingDistance = value; }
        }

        public override bool NeedToRun
        {
            get
            {
                var objects = Client.ObjectSearcher.RetrieveMonsterTargets(i => Client
                .Attributes.ServerPosition.DistanceFrom(i.ServerPosition) < m_CastingDistance);

                foreach (var obj in objects)
                {
                    if (obj.FasInfo != null
                        && obj.FasInfo.FasElapsed)
                        obj.FasInfo = null;
                }

                
                    var copy = new List<MapObject>();
                    lock (objects)
                {
                    //we copy memory here deliberatly!
                    copy = new List<MapObject>(objects);
                    Targets = new List<MapObject>(copy.Where(i => i.FasInfo == null).OrderBy
                        (i => Client.Attributes.ServerPosition.DistanceFrom(i.ServerPosition)));
                    if (m_HavePathRequired)
                        Targets = Targets.Where(i => i.PathToMapObject != null && i.PathToMapObject.Count > 0).ToList();

                    if (Targets.Count > 0)
                        return true;
                }

                return false;
            }
            set
            {

            }
        }

        public override int Priority { get; set; } = 55;

        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled && !InTransition)
            {
                InTransition = true;

                foreach (var obj in Targets)
                {
                    var ReFas = (obj.FasInfo != null && obj.FasInfo.FasElapsed);
                    if (ReFas)
                        obj.FasInfo = null;

                    if (obj.FasInfo == null)
                    {
                        string targetName = obj is Aisling ? ((Aisling)obj).Name : $"Monster_{obj.Serial}";
                        DebugLogger.Log($"FasMonsters: Casting {m_fasSpell} on {targetName}");
                        Client.Utilities.CastSpell(m_fasSpell, obj);
                        break;
                    }
                }

                Client.TransitionTo(this, Elapsed);
            }
        }
    }
}