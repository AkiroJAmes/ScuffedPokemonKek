﻿using System;
using System.Collections.Generic;
using System.Text;

using static AdventureGame.Program;

namespace AdventureGame
{
    abstract class Character : GameObject
    {
        protected string NAME;

        protected int AT;
        protected int MAXHP;
        protected int HP;
        protected int DF;

        public override bool IsAlive()
        {
            return HP <= 0;
        }

        public override int Hp {
            get {
                return this.HP;
            }

            set {
                this.HP = value;
            }
        }

        public virtual int MaxHP { 
            get {
                return MAXHP;
            }
        }

        public override string Name { get { return NAME; } }

        public virtual int At
        {
            get
            {
                return AT;
            }
        }

        public virtual int Df { get
            {
                return DF;
            }
        }
    }

    class Player : Character
    {
        protected int LVL;
        protected int EXP;
        protected int TOTAL_EXP;
        List<GameObject> ITEMS = new List<GameObject>();

        public Player()
        {

        }

        public Player(int at, int hp, int df, int exp, int lvl) {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.EXP = exp;
            this.TOTAL_EXP = exp;
            this.LVL = lvl;
        }

        public int TotalEXP { get { return TOTAL_EXP; } }

        public override int MaxHP
        {
            get
            {
                return CalcStat(MAXHP);
            }
        }

        public override int At
        {
            get
            {
                return CalcStat(AT);
            }
        }

        public override int Df
        {
            get
            {
                return CalcStat(DF);
            }
        }

        int CalcStat(float stat) {
            float exponent = 0.5f;

            return (int)(stat * MathF.Pow(Lvl, exponent));
        }


        public override void Draw() {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("  ");
            Console.ResetColor();
        }

        public int Lvl { get { return LVL; } }

        public int NextLevel() {
            float exponent = 1.5f;
            float baseEXP = 5f;

            // Basic calc for next level
            return (int)Math.Round(baseEXP * (MathF.Pow(Lvl, exponent)));
        }

        public int GetEXP() {
            // Check for level up
            if (EXP >= NextLevel()) {
                EXP -=NextLevel();
                UpdateLVL();
                return EXP;
            }
            return EXP;
        }

        void UpdateLVL() {
            LVL++;
        }

        public void WriteStats(int height) {

            GetEXP();

            string level = $"Level: {Lvl} | EXP: {GetEXP()} / {NextLevel()}";
            int levelPos = 22 - (level.Length / 2);
            string stats = $"HP {Hp} / {MaxHP} : AT {At} : DF {Df}";
            int statPos = 22 - (stats.Length / 2);


            // Make sure the line is clear
            Console.SetCursorPosition(0, height);
            Console.Write("\r                                            ");
            Console.SetCursorPosition(0, height + 1);
            Console.Write("\r                                            ");
            Console.SetCursorPosition(levelPos, height);
            Console.Write(level);
            Console.SetCursorPosition(statPos, height + 1);
            Console.Write(stats);
        }

        public void AddEXP(int exp) {
            // Add exp then check if level up is happening 
            EXP += exp;
            TOTAL_EXP += exp;
            GetEXP();
        }

        public void AddItem(PickUpItem item) {
            for (int i = 0; i < this.ITEMS.Count; i++)
            {
                // Add item to stack with existing items

                if (ITEMS[i].Name == item.Name) {
                    ITEMS[i].Qty = item.Qty + ITEMS[i].Qty;
                    return;
                } 
            }

            this.ITEMS.Add(item);
        }

        public void RemoveItem(GameObject removeItem) {
            foreach (var item in ITEMS)
            {
                if(item.Name == removeItem.Name) {
                    ITEMS.Remove(item);
                    if(gameState == GameState.InInventory)
                        Console.Clear();
                    return;
                }
            }
        }

        public GameObject[] GetItems() { return this.ITEMS.ToArray(); }

        internal void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;
            Console.Write(@"     /\");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop + 1);
            Console.Write(@" __ /`'\ __    /|");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop + 1);
            Console.Write(@"(  /`  '\  )  //");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop + 1);
            Console.Write(@" `'-_  _-'`  //");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop + 1);
            Console.Write(@"  ('    .\('))");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop + 1);
            Console.Write(@"   |._  \ ,./`");
        }
    }

    class Enemy : Character
    {
        protected EnemyBattleAIState AI;
        public Species MySpecies { get; protected set; }
        protected int MyViewDistance;

        public Enemy() { }

        public Enemy(int at, int hp, int df) {
            this.AT = at;
            this.HP = hp;
            this.DF = df;
        }

        public enum Species
        {
            Spider,
            Mimic,
            Rat,
            Skeleton,
            Bat
        }

        public override bool IsEnemy()
        {
            return true;
        }

        public override void Draw() {
            

            if (this.HP > 10) {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("X ");
                Console.ResetColor();
            }
            else {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("x ");
                Console.ResetColor();
             }
        }

        public override EnemyBattleAIState Ai
        {
            get { return AI; }
        }

        public virtual bool ISActive { set { } }

        public virtual int ViewDistance { get { return MyViewDistance; } }

        public virtual void Sprite() {
            Console.Write("");
        }
    }

    class Spider : Enemy
    {
        public Spider(int at, int hp, int df, string name, int viewDistance, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            this.MyViewDistance = viewDistance;
            MySpecies = Species.Spider;
        }

        public Spider(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            this.MyViewDistance = copy.ViewDistance;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("oD");
        }

        public override void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;
            Console.Write(@"'    '   ' '");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@" /`88| '\ \ ");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"     (   )  ");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"      ___   ");
        }
    }

    class Mimic : Enemy
    {
        protected bool IsActive = false;

        public Mimic(int at, int hp, int df, string name, int viewDistance, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            this.MyViewDistance = viewDistance;
            MySpecies = Species.Mimic;
        }

        public Mimic(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            this.MyViewDistance = copy.ViewDistance;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            if (!IsActive) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("? ");
                Console.ResetColor();
            } else {
                Console.Write("[]");
            }
        }

        public override void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;

            Console.Write(@"|___\)___|/");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"|   /(   ||");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@" /_"".':'_/|");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@" \ _VvV__\");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"O_______o");
        }

        public override bool ISActive { set { IsActive = value; } }
    }

    class Rat : Enemy
    {
        public Rat(int at, int hp, int df, string name, int viewDistance, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            this.MyViewDistance = viewDistance;
            MySpecies = Species.Rat;
        }

        public Rat(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            this.MyViewDistance = copy.ViewDistance;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("o/");
        }

        public override void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;
            Console.Write(@""" ` "" """);
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@">' '<  (__.""`");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"(\./)     \_._.-");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"     ,---,");
        }
    }

    class Skeleton : Enemy
    {
        public Skeleton(int at, int hp, int df, string name, int viewDistance, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            this.MyViewDistance = viewDistance;
            MySpecies = Species.Skeleton;
        }

        public Skeleton(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            this.MyViewDistance = copy.ViewDistance;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write("{8");
        }

        public override void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;

            Console.Write(@"  \|_(_(");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@" | | =|= (,");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"(!\_/_|_`\");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@" | | _|._");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"  /|(:. )");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"     ___");
        }
    }

    class Bat : Enemy
    {
        public Bat(int at, int hp, int df, string name, int viewDistance, EnemyBattleAIState ai)
        {
            this.AT = at;
            this.MAXHP = hp;
            this.HP = hp;
            this.DF = df;
            this.NAME = name;
            this.AI = ai;
            this.MyViewDistance = viewDistance;
            MySpecies = Species.Bat;
        }

        public Bat(Enemy copy)
        {
            this.AT = copy.At;
            this.MAXHP = copy.MaxHP;
            this.HP = copy.Hp;
            this.DF = copy.Df;
            this.NAME = copy.Name;
            this.AI = copy.Ai;
            this.MyViewDistance = copy.ViewDistance;
            MySpecies = copy.MySpecies;
        }

        public override void Draw()
        {
            Console.Write(@"\/");
        }

        public override void Sprite()
        {
            var cursorLeftRef = Console.CursorLeft;
            Console.Write(@"  ""-'");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"`\,,/'");
            Console.SetCursorPosition(cursorLeftRef, Console.CursorTop - 1);
            Console.Write(@"_    _");
        }
    }
}
