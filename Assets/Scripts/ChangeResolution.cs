using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    void Start()
    {
        // Limpiar opciones existentes
        dropdown.ClearOptions();

        // Agregar opciones basadas en QualitySettings
        var options = new System.Collections.Generic.List<string>(QualitySettings.names);
        dropdown.AddOptions(options);

        // Establecer la opción actual como la seleccionada
        dropdown.value = QualitySettings.GetQualityLevel();

        // Agregar listener para detectar cambios
        dropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Calidad cambiada a: " + QualitySettings.names[index]);
    }
}
