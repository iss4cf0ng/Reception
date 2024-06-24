using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Shell
{
    public partial class Form1 : Form
    {
        Process cmd_process;
        string prompt;

        private StreamWriter cmdInputWriter;
        private bool inputPromptDisplayed = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CmdOutputDataHandler(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // 如果输入提示符还没有显示，则显示它
                if (!inputPromptDisplayed && e.Data.StartsWith("> "))
                {
                    inputPromptDisplayed = true;
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.AppendText(e.Data + Environment.NewLine);
                    });
                }
                // 如果不是输入提示符，则显示输出
                else if (!e.Data.StartsWith("> "))
                {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.AppendText(e.Data + Environment.NewLine);
                    });
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 创建一个进程对象
            Process process = new Process();

            // 配置进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe"; // 要执行的命令行程序
            startInfo.Arguments = "/c " + textBox2.Text; // 要执行的命令
            startInfo.UseShellExecute = false; // 不使用操作系统外壳程序启动进程
            startInfo.RedirectStandardOutput = true; // 重定向标准输出
            startInfo.RedirectStandardInput = true; // 重定向标准输入
            startInfo.CreateNoWindow = true; // 不创建新窗口
            process.StartInfo = startInfo;

            // 注册事件处理程序以接收命令输出
            process.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);

            // 启动进程
            process.Start();

            // 异步读取标准输出
            process.BeginOutputReadLine();

            // 获取进程的标准输入流
            cmdInputWriter = process.StandardInput;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string cmd = textBox2.Text.Trim();
                if (!string.IsNullOrEmpty(cmd))
                {
                    /*
                    cmd_process.StandardInput.WriteLine(cmd);
                    textBox2.Clear();
                    */

                    var proc = new Process()
                    {
                        StartInfo = new ProcessStartInfo("cmd.exe", "/k " + cmd)
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                        },
                        EnableRaisingEvents = true,
                    };

                    proc.ErrorDataReceived += CmdOutputDataHandler;
                    proc.OutputDataReceived += CmdOutputDataHandler;
                    proc.Start();

                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();

                    proc.WaitForExit();
                }
            }
        }
    }
}