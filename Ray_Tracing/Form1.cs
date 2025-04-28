namespace Ray_Tracing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private View view;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            view = new View();
            view.InitShaders();
        }
    }
}