using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Inputs : Singleton<Inputs> {

    public delegate void Action();
    public delegate void ActionBool(bool i);

    public static event Action OnLeftArrow;
    public static event Action OnRightArrow;
    public static event Action OnUpArrow;
    public static event Action OnDownArrow;
    public static event ActionBool OnUpDownArrow;
    public static event Action OnEnterKey;
    public static event Action OnEscKey;
    public static event Action OnLeftMouseUp;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { OnLeftArrow?.Invoke(); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { OnRightArrow?.Invoke(); }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { OnUpArrow?.Invoke(); OnUpDownArrow?.Invoke(true); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { OnDownArrow?.Invoke(); OnUpDownArrow?.Invoke(false); }
        if (Input.GetKeyDown(KeyCode.Return)) { OnEnterKey?.Invoke(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { OnEscKey?.Invoke(); }
        if (Input.GetMouseButtonUp(0)) { OnLeftMouseUp?.Invoke(); }
    }

}


