using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace exaA0334339
{
    public partial class Form1 : Form
    {
        //宣告參照區

        //bool 布林值
        //只有 true faulse
        private Point player;//玩家座標
        private int grade;//得分數
        private Point mousePos;//滑鼠位置
        private Point[] vMonster;//怪物陣列
        private Random random;

        private Graphics wndGraphics;//視窗畫布
        private Graphics backGraphics;//背景畫布
        private Bitmap backBmp;//點陣圖

       
        private Bullet[] vBullet;//子彈座標


        private const int VIEW_W = 1024;
        private const int VIEW_H = 768;

        private const float MAKE_MONSTER_TIME = 2;
        private float makeMonsterTime;//產生怪物的時間
        private const float MAKE_Bullet_Time = 0.1f;
        private float makeBulletTime;//生子彈的時間

        //預期的畫面更新率(每秒呼叫幾次ontimer)ontimer
        private const float REQUIRE_FPS = 60;
        

        //CONST常數
        private const int MAX_ENEMY = 20;
        private float BALL_SIZE = 25;
        private int blood = 5;//血量
        const int MAX_BULLET = 10;//子彈現有數量

        private Keystate keySpace;//空白鍵
        private Keystate keyUp;//上
        private Keystate keyDown;//下
        private Keystate keyLeft;//左
        private Keystate keyRight;//右

        public Form1()
        {
            InitializeComponent();

            keySpace = new Keystate(Keys.Space);
            keyUp = new  Keystate(Keys.W);
            keyDown = new Keystate(Keys.S);
            keyLeft = new Keystate(Keys.A);
            keyRight = new Keystate(Keys.D);


            makeBulletTime = 0;
            mousePos = new Point();

            makeMonsterTime = MAKE_MONSTER_TIME;

            wndGraphics = CreateGraphics();//建立視窗畫布
            backBmp = new Bitmap(VIEW_W, VIEW_H);//建立點陣圖
            backGraphics = Graphics.FromImage(backBmp);//建立背景畫布

            random = new Random();

          
            vBullet = new Bullet[MAX_BULLET];


            player = new Point();//產生一point物件
            player.x  =460;
            player.y = 650;
           

            vMonster = new Point[MAX_ENEMY];

           

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                vMonster[i] = new Point();

                vMonster[i].x = random.Next(0, VIEW_W);
                vMonster[i].y = random.Next(0, VIEW_H);

            }


            //timer1是計時器+Interval 多久叫一次
            timer1.Interval = 1000 / (int)REQUIRE_FPS;
            timer1.Start();
        }

        private void onPaint(object sender, PaintEventArgs e)
        {
            drawGame();
        }

        private void moveMonster()
        {
            for (int i = 0; i < MAX_ENEMY; i++)
            {
                
                if (vMonster[i] != null)
                {
                    //怪物像玩家一動 移動長度為1
                    vMonster[i].move(player);
                    if (vMonster[i].getdistance(player) < BALL_SIZE + BALL_SIZE)
                    {
                        vMonster[i] = null;
                        blood--;
                        if (blood <=0)//血量少於0 結束
                        Application.Exit();
                        break;
                    }
                }
            }
        }

        private void moveBullet_Killmonster()
        {
            for (int i = 0; i < MAX_BULLET; i++)
            {
                if (vBullet[i] != null)
                {

                    vBullet[i].move();

                    //是否擊中怪物
                    for (int m = 0; m < MAX_ENEMY; m++)
                    {
                        if (vMonster[m] != null)
                        {
                            //子彈與怪物存在
                            //vMonster:Point
                            //vBullet:Bullet
                            //getDistance(傳子類別)
                            if (vMonster[m].getdistance(vBullet[i])
                                < BALL_SIZE + BALL_SIZE)
                            {
                                vBullet[i] = null;
                                vMonster[m] = null;
                                grade++;
                                if (grade >=100)//得分超過100 結束
                                Application.Exit();
                                break;
                            }
                        }
                    }
                    if (vBullet[i] != null)
                    {
                        if (vBullet[i].x > VIEW_W)//出界
                            vBullet[i] = null;
                        else if (vBullet[i].y > VIEW_H)//出界
                            vBullet[i] = null;
                        else if (vBullet[i].x < 0)//出界
                            vBullet[i] = null;
                        else if (vBullet[i].y < 0)//出界
                            vBullet[i] = null;
                    }
                }
            }
        }

        private void drawGame()
        {
             backGraphics.FillRectangle(Brushes.White, 0, 0, VIEW_W, VIEW_H);
            backGraphics.DrawEllipse(Pens.Blue,
                player.x , player.y, BALL_SIZE * 2, BALL_SIZE * 2);

            backGraphics.DrawEllipse(Pens.White, mousePos.x, mousePos.y, 30, 30);

            int total = 0;
            for (int i = 0; i < MAX_BULLET; i++)

                if (vBullet[i] != null)
                {
                    total++;
                    backGraphics.DrawEllipse(Pens.Purple,
                     vBullet[i].x, vBullet[i].y, BALL_SIZE * 2, BALL_SIZE * 2);
                }
            
        
            //子彈數量
            String str = "子彈數量" + total;
            backGraphics.DrawString(str, SystemFonts.CaptionFont,
                                        Brushes.Black, 0, 0);


            //畫遊戲畫面
            total = 0;
            for (int i = 0; i < MAX_ENEMY; i++)
            {
                if (vMonster[i] != null)
                {
                    total++;
                    backGraphics.DrawEllipse(Pens.Red,
                     vMonster[i].x, vMonster[i].y, BALL_SIZE * 2, BALL_SIZE * 2);
                }
            }

            str = "怪物擊殺數量(得分)" +grade;
            backGraphics.DrawString(str, SystemFonts.CaptionFont,
                                        Brushes.Black, 0,20);
            str = "血量" + blood;
            backGraphics.DrawString(str, SystemFonts.CaptionFont,
                                        Brushes.Black, 0, 40);
            str = "上(w),下(s)m左(a)右(d),子彈(空白鍵)";
            backGraphics.DrawString(str, SystemFonts.CaptionFont,
                                        Brushes.Black, 0, 60);

            


            //把背景頁畫到視窗上面
            wndGraphics.DrawImageUnscaled(backBmp, 0, 0);
        }

        //this.inva
        // Invalidate();//通知重繪畫面}

        private void onTimer(object sender, EventArgs e)
        {
            //就是主迴圈main loop
            keySpace.onTimer();//bPress
            keyUp.onTimer();
            keyDown.onTimer();
            keyLeft.onTimer();
            keyRight.onTimer();

            if (keySpace.isPress())//壓下去
            {
                //也要做發射
                addBullet();
            }
            if (keySpace.isDown())
            { 
            //壓著空白鍵
                makeBulletTime -= 1.0f / REQUIRE_FPS;
                if (makeBulletTime <= 0)
                {
                    //子彈發射時間
                    addBullet();
                }
            }
            if (keyUp.isDown())//下
                player.y -= 5;
            if (keyDown.isDown())
                player.y += 5;
            if (keyLeft.isDown())
                player.x  -= 5;
            if (keyRight.isDown())
                player.x  += 5;


            makeMonsterTime -= 1.0f / REQUIRE_FPS; // 1/30秒
            if (makeMonsterTime <= 2.5)
            {
                //產生怪時間到了
                for (int i = 0; i < MAX_ENEMY; i++)
                {
                    if (vMonster[i] == null)
                    {
                        vMonster[i] = new Point();

                        vMonster[i].x = random.Next(0, VIEW_W);
                        vMonster[i].y = random.Next(0, VIEW_H);

                        makeMonsterTime = MAKE_MONSTER_TIME;
                        
                        break;
                    }
                }
               
            }

            moveMonster();

            moveBullet_Killmonster();

            drawGame();

        }

        void addBullet()
        {

            for (int i = 0; i < MAX_BULLET; i++)
            {
                if (vBullet[i] == null)
                {
                    vBullet[i] = new Bullet(player, mousePos);
                    vBullet[i].x = player.x ;
                    vBullet[i].y = player.y;

                    makeBulletTime = MAKE_Bullet_Time;

                 
                    break;
                }
            }
        }
        //滑鼠移動的方法
        private void onMouseMove(object sender, MouseEventArgs e)
        {
            mousePos.x = e.X;
            mousePos.y = e.Y;
        }

        private void onKeyup(object sender, KeyEventArgs e)
        {
            //按鍵放開的通知
            keySpace.onKeyUp(e.KeyCode);
            }
        //類別
        //Point 類別名稱
        //點 用來存放座標資料
        class Point
        {
            public float x, y;//x,y 座標

            //方法成員函式
            public float getdistance(Point pnt)
            {
                float L = (float)Math.Sqrt((x - pnt.x) * (x - pnt.x)
                        + (y - pnt.y) * (y - pnt.y));

                return L;//回傳距離
            }

            //void沒有回傳值
            public void move(Point target)
            {
                float L = getdistance(target);
                float tx, ty;
                if (L > 1)
                {
                    tx = x + (target.x - x) * 1 / L;
                    ty = y + (target.y - y) * 1 / L;
                    x = tx;//設定
                    y = ty;
                }
                else
                {
                    //距離很近
                    x = target.x;
                    y = target.y;
                }
            }
        }

        //繼承
        //定一類別時:Point
        //Bullet 子類別
        //Point 父類別
        class Bullet : Point
        {
            private Point moveDir;//移動向量

            public const float BULLET_SPEED = 10;

            public Bullet(Point pos, Point mousePos)//建構
            {
                x = pos.x;
                y = pos.y;

                float dist = getdistance(mousePos);

                moveDir = new Point();
                moveDir.x = (mousePos.x - x) / dist * BULLET_SPEED;
                moveDir.y = (mousePos.y - y) / dist * BULLET_SPEED;

            }
        public void move()
            {
                //base. 使用父類別的move功能
                x += moveDir.x;
                y += moveDir.y;
            }
        }


        private void onKeyUp(object sender, KeyEventArgs e)
        {
            //按鍵放開的通知
            keySpace.onKeyUp(e.KeyCode);
            keyUp.onKeyUp(e.KeyCode);
            keyDown.onKeyUp(e.KeyCode);
            keyLeft.onKeyUp(e.KeyCode);
            keyRight.onKeyUp(e.KeyCode);
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            //e.KeyChar 按鍵編號
            //if(e.KeyChar==100)
            //檢查(e.KeyChar==100)
            keySpace.onKeyDown(e.KeyCode);
            keyUp.onKeyDown(e.KeyCode);
            keyDown.onKeyDown(e.KeyCode);
            keyLeft.onKeyDown(e.KeyCode);
            keyRight.onKeyDown(e.KeyCode);
        }


      
    //按鍵狀態
    class Keystate
    {
        private Keys theKey;//存放一個對應案件編號

         bool bPress;
         bool bDown;
         bool pPreDown;//上次onTimer是否壓著

        public Keystate(Keys k)//回報剛剛是否按下去
        {
            theKey = k;
            bPress = false;
            bDown = false;
            pPreDown = false;
        }
       
        public bool isPress()//回報是否壓著
        {
            return bPress;
        }

        public void onTimer()//timer通知時呼叫
        {
            //bPress的偵測
            if (bDown == true)//此時是壓著
            {
                if (pPreDown == false)//上次是放開的
                    bPress = true;
                else//上次是壓著的
                    bPress = false;
            }
            else
            {
                //這次是放開的
                bPress = false;
            }

            //把這次的狀態記下來,下次就可以用這個狀態
            pPreDown = bDown;
        }

        public void onKeyDown(Keys k) //偵測
        {
            if (theKey == k)
            {
                bDown = true;
            }
        }
        public void onKeyUp(Keys k) //偵測
        {
            if (theKey == k)
            {
                bPress = false;
                bDown = false;
            }
        }
        public bool isDown()//回報是否壓著
     {
         return bDown;
     }
    };

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void onKeyPress(object sender, KeyPressEventArgs e)
    {

    }
 }
}
