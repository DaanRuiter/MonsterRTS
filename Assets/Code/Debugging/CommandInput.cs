using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class Command : Attribute{}

public class CommandInput : MonoBehaviour {

    public bool use;

    private bool m_focussed;
    private string m_command;
    private string[] m_parameters;
    private InputField m_input;
    private static Text m_output;
    private CanvasGroup m_outputWindow;

    private List<MethodInfo> m_commands;

    private void Start()
    {
        if (!use)
            return;

        m_input = GetComponent<InputField>();
        m_output = GameObject.FindGameObjectWithTag("Command Output").GetComponent<Text>();
        m_outputWindow = transform.parent.GetComponent<CanvasGroup>();

        m_commands = new List<MethodInfo>();
        Assembly[] assemblies  = AppDomain.CurrentDomain.GetAssemblies();

        for (int i = 0; i < assemblies.Length; i++)
        {
            m_commands.AddRange(assemblies[i].GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(Command), false).Length > 0)
                      .ToArray());
        }

    }

    private void Update()
    {

        if (!use)
            return;

        if (Input.GetKeyUp(KeyCode.BackQuote))
            OnFocusGain();

        if(m_focussed)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                m_command = m_input.text;
                m_input.text = "";
                m_input.interactable = false;
                m_parameters = new string[0];

                string[] split = m_command.Split(null);

                m_command = split[0];
                if(split.Length > 1)
                {
                    m_parameters = new string[split.Length - 1];
                    for (int i = 1; i < split.Length; i++)
                    {
                        m_parameters[i - 1] = split[i];
                    }
                }

                MethodInfo method = null;

                for (int i = 0; i < m_commands.Count; i++)
                {
                    if(m_commands[i].Name == split[0])
                    {
                        method = m_commands[i];
                    }
                }

                if (method == null)
                {
                    DebugError(new string[] { "Invalid Command" });
                    return;
                }

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length == 0)
                    method.Invoke(this, null);
                else
                    if (m_parameters[0] == "DebugLog" || m_parameters[0] == "DebugError")
                        OnFocusGain();
                    method.Invoke(this, new object[] {m_parameters});
            }
        }
    }

    public void OnFocusGain()
    {
        if (!use)
            return;

        m_focussed = true;
        m_outputWindow.alpha = 1;
        m_outputWindow.blocksRaycasts = true;
    }

	public void OnFocusLoss()
    {
        if (!use)
            return;

        if (Input.GetKeyUp(KeyCode.Return))
        {
            m_focussed = true;
        }
        else
        {
            m_input.text = "";
            m_focussed = false;
            m_input.interactable = false;
            m_outputWindow.alpha = 0;
            m_outputWindow.blocksRaycasts = false;
        }
    }

    public void OnClick()
    {
        if (!use)
            return;

        m_input.interactable = true;
        m_outputWindow.alpha = 1;
        m_outputWindow.blocksRaycasts = true;
    }

    //debug methods
    [Command]
    public static void DebugLog(string[] text)
    {
        string line = "<color=white>";
        for (int i = 0; i < text.Length; i++)
        {
            if (i == text.Length - 1)
            {
                line += text[i];
            }
            else
            {
                line += text[i] + " ";
            }
        }
        
        m_output.text += line + "</color>\n";
    }

    [Command]
    public static void DebugError(string[] text)
    {
        string line = "<color=red> ! ERROR ! - ";
        for (int i = 0; i < text.Length; i++)
        {
            if (i == text.Length - 1)
            {
                line += text[i];
            }
            else
            {
                line += text[i] + " ";
            }
        }

        m_output.text += line + "</color>\n";
    }

    private static bool l_FOWToggle = true;
    [Command]
    public static void ToggleFOW()
    {
        l_FOWToggle = !l_FOWToggle;
        GameObject[] FOWChunks = GameObject.FindGameObjectsWithTag("Chunk FOW");
        for (int i = 0; i < FOWChunks.Length; i++)
        {
            FOWChunks[i].GetComponent<MeshRenderer>().enabled = l_FOWToggle;
        }
    }

    [Command]
    public static void Spawn(string[] param)
    {
        string identity = param[1];
        EntityType type;

        if(param[0] == "Actor" || param[0] == "Static" || param[0] == "Dynamic")
        {
            type = (EntityType)Enum.Parse(typeof(EntityType), param[0], true);
        }
        else
        {
            DebugLog(new string[] { "Entity Type Not found" });
            return;
        }

        int x = Int32.Parse(param[2]);
        int y = Int32.Parse(param[3]);

        ManagerInstance.Get<EntityManager>().SpawnEntity(identity, type, x, y);
    }

    [Command]
    public void Save(string[] param)
    {
        SaveGame save = new SaveGame(param[0]);
        StartCoroutine(save.Save());
    }

    [Command]
    public void Load(string[] param)
    {
        SaveGame save = SaveGame.LoadSaveFile(param[0]);
        save.LoadSaveIntoGame();
    }
}
