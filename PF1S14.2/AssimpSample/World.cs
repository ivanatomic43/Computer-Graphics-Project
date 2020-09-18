// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL.SceneGraph.Cameras;
using System.Timers;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi



        ///	 Scena koja se prikazuje.
        /// </summary>

        private AssimpScene m_scene;

        private AssimpScene m_2;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = -60.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 700.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;
        private enum TextureObjects { Brick, Floor, Floor1, Floor2, Sveca, Plocice };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images/images.jpg", "..//..//images/714.jpg", "..//..//images/floor1.jpg", "..//..//images/floor2.jpg", "..//..//3D Models/sveca.jpg", "..//..//3D Models/sveca1" +
                ".jpg"};

        private float diffuse0;
        private float diffuse1;
        private float diffuse2;

        public float plateRotation = 0.0f;

        public DispatcherTimer timer1;
        public DispatcherTimer timer2;
        public DispatcherTimer timer3;
        public DispatcherTimer timer4;
        public DispatcherTimer timer5;
        public DispatcherTimer timer6;

        public float scale_plate = 0.0f;

        //animacija
        private float moveCandleX = 0f;
        private float moveCandleY = 0f;
        private float movePlate = 0f;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }

        }

        public float Diffuse0
        {
            get { return diffuse0; }
            set { diffuse0 = value; }
        }
        public float Diffuse1
        {
            get { return diffuse1; }
            set { diffuse1 = value; }
        }
        public float Diffuse2
        {
            get { return diffuse2; }
            set { diffuse2 = value; }
        }
        public float Scale_Plate_Slider {

            get { return scale_plate; }
            set { scale_plate = value; }
        }



        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, String sceneFileName1, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_2 = new AssimpScene(scenePath, sceneFileName1, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];

            diffuse0 = 1f;
            diffuse1 = 0.0f;
            diffuse2 = 0.0f;


        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);

        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Color(0f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            //gl.FrontFace(OpenGL.GL_CW);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Enable(OpenGL.GL_LIGHTING);

            SetupLighting(gl);


            napraviTeksturu(gl);


            m_scene.LoadScene(); //sveca
            m_scene.Initialize(); //tanjir

            m_2.LoadScene();
            m_2.Initialize();

        }
        public void SetTimer1()
        {
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(5);
            timer1.Tick += new EventHandler(MoveCandle1);
            timer1.Start();
        }
        public void SetTimer2() {

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(5);
            timer2.Tick += new EventHandler(MoveCandle2);
            timer2.Start();


        }
        public void MoveCandle1(object sender, EventArgs e)
        {

            moveCandleX += 10f;
            // movePlate += 4.3f;
            if (moveCandleX > 400)
            {
                timer1.Stop();
                SetTimer2();
                //movePlate -= 55;
            }

        }
        public void MoveCandle2(object sender, EventArgs e) {

            moveCandleY += 10;
            if (moveCandleY > 400)
            {

                timer2.Stop();

            }

        }
        
        public void napraviTeksturu(OpenGL gl) //teksture
            { 
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width,image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }

            }





        private void SetupLighting(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            float[] global_ambient = new float[] { 0f, 0f, 0f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0.2f, 0f, 50f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0specular = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);



            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void setUpLightingCandle(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            float[] ambijentalnaKomponenta = { 0f, 0.1f, 0f, 1.0f };
            float[] difuznaKomponenta = { diffuse0, diffuse1, diffuse2, 1.0f };
            float[] spekKomp = { 0.8f, 0.2f, 0f, 1f };
            // Pridruži komponente svetlosnom izvoru 0
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT,
             ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE,
            difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, spekKomp);
            // Podesi parametre tackastog svetlosnog izvora
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);


            gl.Enable(OpenGL.GL_LIGHT0);
            // Pozicioniraj svetlosni izvor
            float[] pozicija = { 0f, 5f, 0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pozicija);


        }



        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            
            gl.PushMatrix();
          // gl.Translate(0, 0, 0);
           //gl.LookAt(0,20,-600 -moveCandle,0,0,-m_sceneDistance, 0,1,0); //kamera

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Viewport(0, 0, m_width, m_height);
            //gl.LookAt(0.0f, 0f, 100f, 0, -1, 0, 0, 1, 0);


            

            //tanjir

            gl.PushMatrix();
            gl.Rotate(0.0f, 0.0f + plateRotation, 0.0f);
            
            
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Plocice]);
            gl.Scale(1.5f + scale_plate, 1.5f  + scale_plate, 1.5f+ scale_plate);
            gl.Translate(52f+ movePlate, -1f + movePlate, 0.2f);
            gl.Rotate(90f, 0f, 0f);
            m_2.Draw();
            gl.PopMatrix();

            //sveca
            
            gl.PushMatrix();
            
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.SVECA]);
            gl.Scale(0.3f, 0.3f, 0.3f);
            gl.Translate(0.2f + moveCandleX, 2f + moveCandleY, 0f);
            setUpLightingCandle(gl);
            gl.Rotate(90f, 0f, 0f);
            m_scene.Draw();
            gl.PopMatrix();

            //podloga
            
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Floor1]);
            gl.Scale(10f, 10f, 10f);
            gl.Color(1f, 1f, 1f);
            // gl.Translate(0f, -3.0f, 0f);
            //gl.Rotate(115f,0f,0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.TexCoord(0.0f, 0f);
            gl.Vertex(-20f, -20f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(20f, -20f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(20f, 20f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(-20f, 20f);
            gl.End();
            gl.PopMatrix();

            //podloga2 zbog cullface
            
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Floor1]);
            gl.Scale(10f, 10f, 10f);
            // gl.Color(0.2f, 0.2f, 0.2f);
            // gl.Translate(0f, -3.0f, 0f);
            //gl.Rotate(115f,0f,0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(-20f, 20f); //obrnut redosled icrtavanja
            gl.TexCoord(1f, 1f);
            gl.Vertex(20f, 20f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(20f, -20f);
            gl.TexCoord(0.0f, 0f);
            gl.Vertex(-20f, -20f);
            gl.End();
            gl.PopMatrix();

            //grid
            gl.PushMatrix();
            Grid grid = new Grid();
            gl.Scale(30f, 30f, 30f);
            //gl.Color(0.1f, 0.1f, 0.1f);
            //gl.Translate(0f, -1.0f, 0f);
            //gl.Rotate(115f, 0f, 0f);
            grid.Render(gl, RenderMode.Design);
            gl.PopMatrix();

            //zid iza

            gl.PushMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.Color(0.81f, 0.82f, 0.81f);
            Cube iza = new Cube();
            gl.Scale(200f, 5f, 60f);
            gl.Normal(0f, 1f, 0f);
            gl.Translate(0f, 40f, 1f);
            //gl.Rotate(90f, 0f, 0f);
            
            iza.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            /*zid ispred ako zatreba
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            gl.Color(0.81f, 0.82f, 0.81f);
            Cube ispred= new Cube();
            gl.Scale(200f, 5f, 60f);
            gl.Translate(0f, -40f, 1f);
            //gl.Rotate(90f, 0f, 0f);
            ispred.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            */

            //zid levi
            
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            //gl.Color(0.81f, 0.82f, 0.81f);
            Cube levi = new Cube();
            gl.Scale(5f, 205f, 60f);
            gl.Normal(0f, 1f, 0f);
            gl.Translate(-40f, 0.02f, 1f);
            //gl.Rotate(90f, 0f, 0f);
            levi.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            //zid desni
            
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
            //gl.Color(0.81f, 0.82f, 0.81f);
            Cube desni = new Cube();
            gl.Scale(5f, 205f, 60f);
            gl.Normal(0f, 1f, 0f);
            gl.Translate(40f, 0.02f, 1f);
            //gl.Rotate(90f, 0f, 0f);
            desni.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            
            //tekst

            //gl.PushMatrix();
            crtajtekst(gl);
            //gl.PopMatrix();
            
            gl.PopMatrix();
            gl.Flush();
        }
        public void crtajtekst(OpenGL gl){
            
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Viewport(m_width/2,0, m_width / 2, m_height / 2);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(-10.0f, 10.0f, -10.0f, 10.0f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            
            //ispis teksta 
            gl.PushMatrix();
            
            gl.Color(0.0f, 0f, 1.0f);

            gl.PushMatrix();
            gl.Translate(-1f, -4.0f, 0.0f);
            gl.DrawText3D("Times New Roman", 12f, 0.0f, 0.0f, "Predmet: Racunarska grafika");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -4.1f, 0f);
            gl.DrawText3D("Times New Roman", 12f, 1f, 0.1f, "_______________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -5.0f, 0.0f);
            gl.DrawText3D("Times New Roman", 12f, 0.0f, 0.0f, "Sk.god: 2018/19");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -5.1f, 0f);
            gl.DrawText3D("Times New Roman", 12f, 1f, 0.1f, "_______________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -6.0f, 0.0f);
            gl.DrawText3D("Times New Roman", 12f, 0.0f, 0.0f, "Ime: Ivana");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -6.1f, 0f);
            gl.DrawText3D("Times New Roman", 12f, 1f, 0.1f, "_______________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -7.0f, 0.0f);
            gl.DrawText3D("Times New Roman", 12f, 0.0f, 0.0f, "Prezime: Tomic");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -7f, 0f);
            gl.DrawText3D("Times New Roman", 12f, 1f, 0.1f, "_______________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -8.0f, 0.0f);
            gl.DrawText3D("Times New Roman", 12f, 0.0f, 0.0f, "Sifra zad: 14.2");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(-1f, -8f, 0f);
            gl.DrawText3D("Times New Roman", 12f, 1f, 0.1f, "_______________________");
            gl.PopMatrix();
            SetupLighting(gl);

            gl.PopMatrix();
            
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.Perspective(45f, 2f, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.PopMatrix();
            


        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }
        

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
