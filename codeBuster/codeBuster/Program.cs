using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Send your busters out into the fog to trap ghosts and bring them home!
 **/
class Player
{
    public enum todo
    {
        empty = 0,
        tobust = 1,
        intarget = 2,
        released = 3,
        missed = 4
    }

    public enum job
    {
        move = 8,
        toRelease = 2,
        bust = 3,
        nothing = 7,
        hunt = 5,
        stun = 0,
        kill = 1,
        scout = 4
    }
    class Enemy
    {
        public int id;
        public int x;
        public int y;
        public int state;
        public int value;
        public int entityType;
        public todo situation;
        public Enemy(int id, int x, int y, int state, int value, int entityType)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.state = state;
            this.value = value;
            this.entityType = entityType;
            if (entityType == -1)
                newghost();

        }
        public void newghost()
        {
            bool contain = false;
            int idIndx = -1;
            for (int i = 0; i < ghost.Count; i++)
            {
                if (ghost[i].id == this.id)
                {
                    contain = true;
                    idIndx = i;
                }
            }
            if (contain == false)
            {
                ghost.Add(this);
            }
            else
            {

                ghost[idIndx].x = this.x;
                ghost[idIndx].y = this.y;
                ghost[idIndx].state = this.state;
                ghost[idIndx].value = this.value;
                if (ghost[idIndx].situation == todo.missed)
                    ghost[idIndx].situation = todo.intarget;

            }
        }
    }

    class Base
    {
        public int x;
        public int y;

        public Base(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    class Buster
    {
        public int id;
        public int x;
        public int y;
        public int state;
        public int value;
        public job bJob;
        public string doing;
        public string target;
        public int charge;
        public int steal = -1;
        public string tarcoor;
        public Enemy gh;


        public Buster(int id, int x, int y, int state, int value)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.state = state;
            this.value = value;

        }
        public void check()
        {

            give();
            hunting();
            searching();
            if (firsTask == false)
                firstScout();
        }
        public void give()
        {

            counter++;
            for (int i = 0; i < worker.Count; i++)
            {
                worker[i].charge++;
                if (worker[i].state == 2)
                {
                    worker[i].bJob = job.stun;
                    if (worker[i].gh != null)
                    {
                        worker[i].gh.situation = todo.intarget;
                        worker[i].gh = null;
                    }

                }
                if (worker[i].state == 1)
                {
                    worker[i].doing = "MOVE";
                    worker[i].target = home.x.ToString() + " " + home.y.ToString();
                    worker[i].bJob = job.toRelease;
                }
                var toswitch = worker[i].bJob;
                switch (toswitch)
                {
                    case job.bust:
                        {

                            if (worker[i].state == 3)
                            {
                                worker[i].bJob = job.bust;
                            }
                            else
                            {
                                worker[i].bJob = job.nothing;
                                worker[i].gh = null;
                            }
                            break;
                        }
                    case job.move:
                        {
                            string[] distx = (worker[i].target.Split(' '));
                            if (distance(Convert.ToInt32(distx[0]), Convert.ToInt32(distx[1]), worker[i].x, worker[i].y) < 800)
                            {
                                worker[i].bJob = job.nothing;
                                if (worker[i].gh != null)
                                {
                                    worker[i].gh.situation = todo.missed;
                                    worker[i].gh = null;

                                }
                            }
                            else
                                worker[i].bJob = job.move;
                            break;
                        }
                    case job.nothing:
                        {
                            if (worker[i].state == 1)
                            {
                                worker[i].doing = "MOVE";
                                worker[i].target = home.x.ToString() + " " + home.y.ToString();
                                worker[i].bJob = job.toRelease;
                            }
                            else
                            {
                                worker[i].bJob = job.nothing;
                            }
                            break;
                        }

                    case job.toRelease:
                        {
                            if (distance(worker[i].x, worker[i].y, home.x, home.y) < 1600)
                            {
                                worker[i].doing = "RELEASE";
                                worker[i].target = "";
                                worker[i].bJob = job.toRelease;
                                if (worker[i].gh != null)
                                {
                                    worker[i].gh.situation = todo.released;
                                    ghost.Remove(worker[i].gh);
                                }
                                worker[i].gh = null;
                            }
                            else
                                worker[i].bJob = job.toRelease;
                            if (worker[i].state == 0)
                            {
                                worker[i].bJob = job.nothing;

                            }
                            break;
                        }

                    case job.stun:
                        {
                            if (worker[i].state != 2)
                            {
                                worker[i].bJob = job.nothing;

                                worker[i].gh = null;

                            }
                            break;
                        }
                    case job.kill:
                        {
                            bool isee = false;
                            if (worker[i].steal != -1)
                            {
                                if (worker[i].gh != null)
                                {
                                    worker[i].gh.situation = todo.intarget;
                                }
                                worker[i].gh = null;
                                for (int j = 0; j < insight.Count; j++)
                                {
                                    if (insight[j].entityType == -1 && worker[i].steal.Equals(insight[j].id))
                                    {
                                        if (distance(worker[i].x, worker[i].y, insight[j].x, insight[j].y) < 1760)
                                        {
                                            worker[i].bJob = job.kill;
                                            worker[i].doing = "BUST";
                                            insight[j].situation = todo.tobust;
                                            worker[i].target = worker[i].steal.ToString();
                                            worker[i].steal = -1;
                                            isee = true;
                                        }
                                        else
                                        {
                                            worker[i].bJob = job.bust;
                                            worker[i].doing = "MOVE";
                                            worker[i].target = insight[j].x.ToString() + " " + insight[j].y.ToString();
                                            worker[i].steal = -1;
                                            isee = true;
                                        }
                                    }
                                }
                                if (isee == false)
                                {
                                    worker[i].bJob = job.bust;
                                    worker[i].doing = "MOVE";
                                    worker[i].target = worker[i].tarcoor;
                                    worker[i].steal = -1;
                                    worker[i].tarcoor = null;
                                }


                            }
                            else
                            {
                                worker[i].bJob = job.nothing;

                            }
                            break;
                        }
                    case job.scout:
                        if (firsTask == true)
                        {
                            worker[i].bJob = job.nothing;

                        }
                        break;

                }
            }
        }
        public void hunting()
        {
            if (insight.Count != 0)
            {
                insight = insight.OrderBy(item => item.state).ToList();
                for (int i = 0; i < insight.Count; i++)
                {
                    if (insight[i].situation == todo.empty)
                        insight[i].situation = todo.intarget;
                    int indx = -1;
                    int minDist = int.MaxValue;
                    for (int j = 0; j < worker.Count; j++)
                    {
                        if (insight[i].entityType == -1 && insight[i].situation != todo.released)
                        {
                            if (worker[j].bJob > (job)4 || worker[j].bJob == job.scout && insight[i].state < 4)
                            {
                                //if (worker[j].gh == null || worker[j].gh.id == insight[i].id)
                                {
                                    int temp = distance(insight[i].x, insight[i].y, worker[j].x, worker[j].y);
                                    if (minDist > temp)
                                    {
                                        minDist = temp;
                                        indx = j;
                                    }
                                }
                            }

                        }
                        else if (insight[i].state != 2)
                        {
                            if (worker[j].bJob > (job)0 && worker[j].charge > 19)
                            {
                                int temp = distance(insight[i].x, insight[i].y, worker[j].x, worker[j].y);
                                if (minDist > temp)
                                {
                                    minDist = temp;
                                    indx = j;
                                }
                            }

                        }

                    }
                    //if (indx != -1)
                    //Console.Error.WriteLine("typ: " + insight[i].entityType + " enid: "
                    //    + insight[i].id + "wid: " + worker[indx].id + "dist " + minDist + "job: " + worker[indx].bJob+ worker[indx].gh.id);
                    if (minDist < 1760 && indx != -1)
                    {
                        if (insight[i].entityType == -1)
                        {
                            if (minDist < 900)
                            {
                                worker[indx].doing = "MOVE";
                                worker[indx].target = home.x.ToString() + " " + home.y.ToString();
                                worker[indx].bJob = job.move;
                                if (worker[indx].gh != null && worker[indx].gh != insight[i])
                                    worker[indx].gh.situation = todo.intarget;
                                worker[indx].gh = ghost.Find(item => item.id == insight[i].id);
                            }
                            else
                            {
                                worker[indx].bJob = job.bust;
                                worker[indx].doing = "BUST";
                                worker[indx].target = insight[i].id.ToString();
                                if (worker[indx].gh != null && worker[indx].gh != insight[i])
                                    worker[indx].gh.situation = todo.intarget;
                                worker[indx].gh = ghost.Find(item => item.id == insight[i].id);
                            }
                        }
                        else if (worker[indx].charge > 19 && insight[i].state != 2 && insight[i].state != 0
                            || worker[indx].charge > 19 && insight[i].state != 2 && worker[indx].state == 3)
                        {
                            if (insight[i].state == 1)
                            {
                                worker[indx].steal = insight[i].value;
                                worker[indx].tarcoor = insight[i].x.ToString() + " " + insight[i].y.ToString();
                                worker[indx].bJob = job.kill;
                                worker[indx].doing = "STUN";
                                worker[indx].target = insight[i].id.ToString();
                                worker[indx].charge = 0;

                            }
                            else
                            {
                                worker[indx].bJob = job.kill;
                                worker[indx].doing = "STUN";
                                worker[indx].target = insight[i].id.ToString();
                                worker[indx].charge = 0;
                            }
                        }

                    }
                    else if (indx != -1)
                    {
                        if (insight[i].entityType == -1)
                        {
                            if (worker[indx].bJob != job.toRelease && worker[indx].bJob != job.scout)
                            {

                                worker[indx].bJob = job.move;
                                worker[indx].doing = "MOVE";
                                worker[indx].target = insight[i].x.ToString() + " " + insight[i].y.ToString();
                                if (worker[indx].gh != null && worker[indx].gh != insight[i])
                                    worker[indx].gh.situation = todo.intarget;
                                worker[indx].gh = ghost.Find(item => item.id == insight[i].id);

                            }
                        }
                        else if (insight[i].state == 1 && worker[indx].bJob != job.toRelease)
                        {
                            if (home.x < 2000)
                            {
                                worker[indx].doing = "MOVE";
                                worker[indx].target = "15000" + " " + "8000";
                                worker[indx].bJob = job.move;
                            }
                            else
                            {
                                worker[indx].doing = "MOVE";
                                worker[indx].target = "1000" + " " + "800";
                                worker[indx].bJob = job.move;
                            }

                        }
                    }
                }
                for (int i = 0; i < worker.Count; i++)
                {
                    if (worker[i].gh != null)
                    {
                        if (worker[i].bJob == job.toRelease)
                            ghost.Find(x => x.id == worker[i].gh.id).situation = todo.released;
                        else if (worker[i].gh.situation != todo.released)
                            ghost.Find(x => x.id == worker[i].gh.id).situation = todo.tobust;

                        Console.Error.WriteLine(worker[i].id + " id " + worker[i].gh.id + " ghid " + worker[i].gh.situation);
                    }
                }

            }

        }
        public void searching()
        {
            stepper = 0;
            bool haveAnygh = false;
            ghost = ghost.OrderBy(o => o.state).ToList();
            for (int i = 0; i < ghost.Count; i++)
            {
                Console.Error.WriteLine(ghost[i].id + " id " + ghost[i].situation + " szitu " + ghost[i].state + " state " + ghost[i].x + " x " + ghost[i].y + " y ");
            }

            List<string> ghTarget = ghost.FindAll(item => item.situation == todo.intarget)
                .Select(xx => xx.x.ToString() + " " + xx.y.ToString()).ToList();
            string[] objectives;
            bool ok = false;
            if (ghTarget.Count != 0)
            {
                objectives = ghTarget.ToArray();
                haveAnygh = true;
            }
            else
            {
                if (home.x < 10)
                    objectives = new string[] { "14500 5500", "12500 5500", "9500 4500", "14500 7500", "11500 7500", "8000 4500" };
                else
                    objectives = new string[] { "4000 1100", "3000 2500", "4000 4500", "1500 3500", "3500 3500", "8000 4500" };
            }
            for (int i = 0; i < worker.Count; i++)
            {
                ok = false;
                if (worker[i].bJob == job.nothing && counter < 150)
                {

                    worker[i].doing = "MOVE";
                    worker[i].bJob = job.move;
                    if (stepper > objectives.Length - 1)
                        stepper = 0;
                    worker[i].target = objectives[stepper];
                    if (haveAnygh == true)
                        worker[i].gh = ghost.Find(c => c.x.ToString() + " " + c.y.ToString() == (worker[i].target));
                    else
                    {
                        worker[i].gh = null;
                    }
                    stepper++;

                }
                else if (worker[i].bJob == job.nothing && counter > 149)
                {
                    for (int j = 0; j < worker.Count; j++)
                    {
                        if (worker[j].state == 1 && worker[i].charge > 19)
                        {
                            worker[i].doing = "MOVE";
                            worker[i].bJob = job.move;
                            worker[i].target = worker[j].x.ToString() + " " + worker[j].y.ToString();
                            ok = true;
                        }
                    }
                    if (worker[i].bJob == job.nothing && ok == false)
                    {
                        if (stepper > objectives.Length - 1)
                            stepper = 0;
                        worker[i].doing = "MOVE";
                        worker[i].bJob = job.move;
                        worker[i].target = objectives[stepper];
                        worker[i].gh = ghost.Find(c => c.x.ToString() + " " + c.y.ToString() == (worker[i].target));
                        stepper++;

                    }
                }
            }
        }
        public void firstScout()
        {
            if (counter < 2)
            {
                int tempdist = 4500;
                int tempindx;
                for (int i = 0; i < worker.Count; i++)
                {
                    if (home.x < 4500)
                    {
                        if (worker[i].x < tempdist)
                        {
                            tempdist = worker[i].y;
                            tempindx = i;
                        }
                    }
                    else
                    {
                        if (worker[i].x > tempdist)
                        {
                            tempdist = worker[i].y;
                            tempindx = i;
                        }
                    }
                }
            }

            string[] objectives;
            if (home.x < 2000)
            {
                objectives = new string[] { "9000 2000", "8000 4500", "1000 8000" };
            }
            else
            {
                objectives = new string[] { "7000 7000", "8000 4500", "15000 1000" };
            }
            if (worker[0].bJob != job.kill && worker[0].bJob != job.bust && worker[0].bJob != job.toRelease)
            {
                worker[0].bJob = job.scout;
                worker[0].doing = "MOVE";
                worker[0].target = objectives[fscout];
            }
            if (worker[0].doing == "MOVE")
            {
                string[] distx = (worker[0].target.Split(' '));
                if (distance(Convert.ToInt32(distx[0]), Convert.ToInt32(distx[1]), worker[0].x, worker[0].y) < 800)
                {
                    fscout++;
                    if (fscout == objectives.Length)
                        firsTask = true;
                }
            }
            Console.Error.WriteLine(worker[0].bJob);
        }
        public void trigo(int x, int y, int xtarget, int ytarget)
        {
            int xh = home.x > 10 ? 1 : 16000;
            int yh = home.x > 10 ? 1 : 9000;
            int cside = distance(xtarget, ytarget, xh, yh);
            int aside = distance(x, y, xh, yh);
            int bside = distance(x, y, xtarget, ytarget);
            if (cside / 800 >= (aside - 1700) / 800)
            {
                double beta = (Math.Acos(Math.Pow(aside, 2) + Math.Pow(cside, 2) - Math.Pow(bside, 2)
                    / (2 * aside * bside)) / Math.PI) * 180;
                double gamma = 45;
                int vaside = aside / 2;


            }
        }
    }


    static int distance(int x, int y, int x1, int y1)
    {
        int distance = (int)Math.Sqrt(Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2));

        return distance;
    }
    static Random r = new Random();
    static bool firsTask = false;
    static List<Buster> worker = new List<Buster>();
    static List<Enemy> insight = new List<Enemy>();
    static List<Enemy> ghost = new List<Enemy>();
    static int nrOfBust;
    static Base home;
    static int counter;
    static int fscout = 0;
    static int stepper = 0;
    static void Main(string[] args)
    {
        counter = 0;
        bool first = false;
        int bustersPerPlayer = int.Parse(Console.ReadLine()); // the amount of busters you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right

        if (myTeamId == 0)
            home = new Base(0, 0);
        else
            home = new Base(15999, 8999);
        nrOfBust = bustersPerPlayer;
        // game loop
        while (true)
        {

            int entities = int.Parse(Console.ReadLine()); // the number of busters and ghosts visible to you
            for (int i = 0; i < entities; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int entityId = int.Parse(inputs[0]); // buster id or ghost id
                int x = int.Parse(inputs[1]);
                int y = int.Parse(inputs[2]); // position of this buster / ghost
                int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
                int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
                int value = int.Parse(inputs[5]); // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.
                if (entityType == myTeamId)
                {

                    if (first == false)
                    {
                        Buster b = new Buster(entityId, x, y, state, value);
                        b.bJob = job.nothing;
                        b.charge = 20;
                        worker.Add(b);
                        //idle.Add(b);
                    }
                    else
                    {
                        for (int j = 0; j < worker.Count; j++)
                        {
                            if (worker[j].id == entityId)
                            {
                                worker[j].x = x;
                                worker[j].y = y;
                                worker[j].value = value;
                                worker[j].state = state;
                            }
                        }

                    }
                }
                else
                {
                    Enemy g = new Enemy(entityId, x, y, state, value, entityType);

                    insight.Add(g);
                }
            }
            worker[1].check();
            // Console.Error.WriteLine(idle.Count+"");
            for (int i = 0; i < worker.Count; i++)
            {

                // Write an action using Console.WriteLine()
                //  Console.Error.WriteLine("Debug messages...");
                Console.WriteLine(worker[i].doing + " " + worker[i].target);
                // Console.Error.WriteLine(worker[i].bJob+" doing: "+ worker[i].doing + " " + worker[i].target);
            }
            insight.Clear();
            first = true;
        }
    }
}