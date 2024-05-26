using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Threading;

public class ComController : MonoBehaviour
{
    public static SerialPort spCom;
    public Dropdown dropBoxPort;
    public Text lbMsg;
    public GameController gameController;

    public int readTimeOut = 500; // wait time out : 500ms
    public int frameShowWhiteCircle = 8; //total frame show circleBlock
    public int frameShowBlackScreen = 20; // total frame show black green

    bool isReadSensor = false;
    int rx;
    int lastKey = -1;
    int totalFps = 0;
    int sensorFps = 0;

    //VER SI SE OPTIMIZA
    Thread serialThread;
    bool keepReading = true;

    // Start is called before the first frame update
    void Start()
    {
        lbMsg.text = "";

        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            dropBoxPort.options.Add(new Dropdown.OptionData(port));
        }
    }

    // Create A Port
    public void CreatePort()
    {
        string value = dropBoxPort.options[dropBoxPort.value].text;
        StartPort(value);
    }

    // Start a Serial Port
    public void StartPort(string pName)
    {
        spCom = new SerialPort(pName, 9600);
        bool isOK = false;
        if (spCom != null)
        {
            if (!spCom.IsOpen)
            {
                try
                {
                    spCom.Open();
                    spCom.ReadTimeout = readTimeOut; // wait time out : 500ms
                    if (spCom.ReadByte() > 0)
                    {
                        gameController.StartGame(); // start game
                        isOK = true;
                        Debug.Log(pName + " OPENED!");
                    }
                }
                catch
                {
                    lbMsg.text = "PORT IS NOT READY!";
                }
            }
        }

        if (!isOK)
            lbMsg.text = "PORT IS NOT READY!";


    }

    //VER SI SE OPTIMIZA X2
    void ReadSerial()
    {
        while (keepReading && spCom != null && spCom.IsOpen)
        {
            try
            {
                string s = spCom.ReadLine();
                int value;
                if (int.TryParse(s, out value))
                {
                    lock (this)
                    {
                        rx = value;
                    }
                }
            }
            catch
            {
                // Handle read exceptions
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (gameController.gameStatus < 0)
            return;

        //OTRA OPTIMIZACION
        int currentRx;
        lock (this)
        {
            currentRx = rx;
        }
        //AAAAAA
        if (spCom.IsOpen)
        {
            string s = spCom.ReadLine();
            try
            {
                rx = int.Parse(s); // 9: trigger press begin, 10: trigger press end,  0: sensor true , 1 : sensor false
            }
            catch
            {
                return;
            }
            if (rx == 9 && !isReadSensor) // trigger press
            {
                totalFps = 0;
                sensorFps = 0;
                isReadSensor = true;
                gameController.ShootBegin(); // show black screen and hide white circle
            }
            else if (rx < 9 && isReadSensor) // Read sensor state
            {
                lastKey = rx; // save last state of sensor (1 or 0)
                totalFps++;
                sensorFps += rx; // count state sensor

                if (totalFps == frameShowWhiteCircle) // show white circle after 8 frame
                {
                    gameController.whiteBlock.SetActive(true);
                }
                if (totalFps > frameShowBlackScreen) // hide black screen after 20 frame
                {

                    isReadSensor = false; // stop read sensor state
                    gameController.HideBlackScreen();
                    Debug.Log(sensorFps + "/" + totalFps);
                    if (lastKey == 0 && sensorFps > 0) // last state of sensor is 0 : hit the white circle target.  sensorFps > 0 (ignore other lights)
                    {
                        gameController.OnDamage();
                        Debug.Log("On Damage!");
                    }
                    totalFps = 0;
                    sensorFps = 0;
                }
            }
        }
    }

    //MAS OPT9IMIZAICON
    void OnApplicationQuit()
    {
        keepReading = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }
        if (spCom != null && spCom.IsOpen)
        {
            spCom.Close();
        }
    }
}
