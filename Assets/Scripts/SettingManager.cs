using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;
using SFB;

[System.Serializable]
public class Parameters
{
	public string name = "";
	public string value = "";
}

[System.Serializable]
public class configParams
{
	public Parameters[] parameters;
}

public class SettingManager : MonoBehaviour
{
	// Start is called before the first frame update
	[SerializeField] ColorPicker backColor;
	[SerializeField] ColorPicker objColor;

	[SerializeField] Button btnBack;
	[SerializeField] Button btnObj;

	[SerializeField] InputField numOfCount;
	[SerializeField] InputField ceiling;
	[SerializeField] InputField depth;
	[SerializeField] InputField oswait;
	[SerializeField] InputField thickness;
	[SerializeField] Dropdown objNo;
	[SerializeField] Dropdown objDir;
	[SerializeField] InputField objSpeed;
	[SerializeField] InputField back_R;
	[SerializeField] InputField back_G;
	[SerializeField] InputField back_B;
	[SerializeField] InputField obj_R;
	[SerializeField] InputField obj_G;
	[SerializeField] InputField obj_B;

	private Rect windowRect = new Rect ((Screen.width - 200)/2, (Screen.height - 300)/2, 200, 300);

	string[] dir, color, speed;
	string back_color;
	Parameters[] json;


	string configureFile = System.IO.Path.GetFullPath("config.json");

	void Start()
	{
		int num = 0;
		dir = new string[1000];
		color = new string[1000];
		speed = new string[1000];
		json = new Parameters[3000];

		for(int i = 0; i < 1000; i++)
		{
			color[i] = "00FFFFFF";
			dir[i] = "down";
			speed[i] = "2000";
		}

		for(int i = 0; i < 3000; i++)
		{
			json[i] = new Parameters();
		}


		string config = File.ReadAllText(configureFile, Encoding.UTF8);
		configParams root = JsonUtility.FromJson<configParams>(config);
		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "NumOfCounter")
			{
				num = int.Parse(root.parameters[i].value);
				numOfCount.text = num.ToString();
				break;
			}
		}
		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "Depth")
			{
				depth.text = root.parameters[i].value;
				break;
			}
		}

		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "Ceiling")
			{
				ceiling.text = root.parameters[i].value;
				break;
			}
		}

		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "Background")
			{
				back_color = root.parameters[i].value;
				GetBackColor(backColor);
				break;
			}
		}

		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "Thickness")
			{
				thickness.text = root.parameters[i].value;
				break;
			}
		}

		for(int i = 0; i < root.parameters.Length; i++)
		{
			if(root.parameters[i].name == "OSWait")
			{
				oswait.text = root.parameters[i].value;
				break;
			}
		}
		for(int i = 0; i < num; i++)
		{
			string param_str = "Color" + i.ToString();
			int j;

			for(j = 0; j < root.parameters.Length; j++)
			{
				if(root.parameters[j].name == param_str) {
					color[i] = root.parameters[j].value;
					break;
				}
			}

			param_str = "Speed" + i.ToString();
			for(j = 0; j < root.parameters.Length; j++)
			{
				if(root.parameters[j].name == param_str) {
					speed[i] = root.parameters[j].value;
					break;
				}
			}
			
			param_str = "Direction" + i.ToString();
			for(j = 0; j < root.parameters.Length; j++)
			{
				if(root.parameters[j].name == param_str) {
					dir[i] = root.parameters[j].value;
					break;
				}
			}
		}
		OnCountChange();

		// backColor.Title = "Background color...";
		// backColor.startPos = new Vector2(320, 70);
		// backColor.colorSetFunctionName = "SetBackColor";
		// backColor.colorGetFunctionName = "GetBackColor";
		// backColor.index = 0;
		// backColor.drawOrder = 1;
		backColor.receiver = this.gameObject;
		objColor.receiver = this.gameObject;
		backColor.gameObject.SetActive(false);
		objColor.gameObject.SetActive(false);
		OnIndexChange();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void OnSelectBackColor()
	{
		backColor.gameObject.SetActive(true);
		backColor.setShowing();
	}

	public void OnSelectObjColor()
	{
		objColor.gameObject.SetActive(true);
		objColor.setShowing();
	}

	public void SetBackColor(ColorInfo ci)
	{
		int r = (int)(ci.color.r * 255.0f);
		int g = (int)(ci.color.g * 255.0f);
		int b = (int)(ci.color.b * 255.0f);
		back_color = "00" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
		Debug.Log(back_color);
		SaveSetting();
		back_R.text = r.ToString();
		back_G.text = g.ToString();
		back_B.text = b.ToString();
		// txtBackColor.text = back_color;
		btnBack.GetComponent<Image>().color = ci.color;
		// backColor.gameObject.SetActive(false);
	}

	public void GetBackColor(ColorPicker picker)
	{
		int color_int = 0;
		color_int = int.Parse(back_color, System.Globalization.NumberStyles.HexNumber);
		float r = (float)((color_int & 0xFF0000) >> 16);
		float g = (float)((color_int & 0xFF00) >> 8);
		float b = (float)((color_int & 0xFF));
		back_R.text = ((int)r).ToString();
		back_G.text = ((int)g).ToString();
		back_B.text = ((int)b).ToString();
		r /= 255.0f;
		g /= 255.0f;
		b /= 255.0f;
		picker.NotifyColor(new Color(r,g,b));
		// txtBackColor.text = back_color;
		btnBack.GetComponent<Image>().color = new Color(r,g,b);
	}

	public void SetObjColor(ColorInfo ci)
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		int r = (int)(ci.color.r * 255.0f);
		int g = (int)(ci.color.g * 255.0f);
		int b = (int)(ci.color.b * 255.0f);
		color[idx] = "00" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
		Debug.Log(color[idx]);
		SaveSetting();
		obj_R.text = r.ToString();
		obj_G.text = g.ToString();
		obj_B.text = b.ToString();
		// txtObjColor.text = color[idx];
		btnObj.GetComponent<Image>().color = ci.color;
	}

	public void GetObjColor(ColorPicker picker)
	{
		int color_int = 0;
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		color_int = int.Parse(color[idx], System.Globalization.NumberStyles.HexNumber);
		float r = (float)((color_int & 0xFF0000) >> 16);
		float g = (float)((color_int & 0xFF00) >> 8);
		float b = (float)((color_int & 0xFF));
		obj_R.text = ((int)r).ToString();
		obj_G.text = ((int)g).ToString();
		obj_B.text = ((int)b).ToString();
		r /= 255.0f;
		g /= 255.0f;
		b /= 255.0f;
		picker.NotifyColor(new Color(r,g,b));
		// txtObjColor.text = color[idx];
		btnObj.GetComponent<Image>().color = new Color(r,g,b);
	}

	public void SaveSetting()
	{
		int num = 0;
		num = int.Parse(numOfCount.text);
		json[0].name = "NumOfCounter"; json[0].value = numOfCount.text;
		json[1].name = "Depth"; json[1].value = depth.text;
		json[2].name = "Ceiling"; json[2].value = ceiling.text;
		json[3].name = "Background"; json[3].value = back_color;
		json[4].name = "Thickness"; json[4].value = thickness.text;
		json[5].name = "OSWait"; json[5].value = oswait.text;
		for(int i = 0; i < num; i++)
		{
			json[i*3+6].name = "Color" + i.ToString(); json[i*3+6].value = color[i];
			json[i*3+7].name = "Speed" + i.ToString(); json[i*3+7].value = speed[i];
			json[i*3+8].name = "Direction" + i.ToString(); json[i*3+8].value = dir[i];
		}
		string jsonString = "";
		for(int i = 0; i < 6 + num * 3; i++)
		{
			if(i == 0){
				jsonString = JsonUtility.ToJson(json[i]);
			}
			else 
			{
				jsonString += ",\n" + JsonUtility.ToJson(json[i]);
			}
		}
		jsonString = "{\n\"parameters\":[\n" + jsonString + "\n]\n}";
		// Debug.Log(jsonString);
		File.WriteAllText(configureFile, jsonString);
	}

	public void OnCountChange()
	{
		// Debug.Log("Count change...");
		int cnt = int.Parse (numOfCount.text);
		List<string> temp = new List<string>();
		objNo.ClearOptions();
		for(int i = 0; i < cnt; i++)
		{
			temp.Add(i.ToString());
		}
		objNo.AddOptions(temp);
		SaveSetting();
	}

	public void OnIndexChange()
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		objDir.captionText.text = dir[idx];
		// objColor.text = color[idx];
		GetObjColor(objColor);
		objSpeed.text = speed[idx];
	}

	public void OnBackColorChange()
	{
		// backColor.gameObject.SetActive (true);
		// SaveSetting();
	}

	public void OnCeilChange()
	{
		SaveSetting();
	}

	public void OnDepthChange()
	{
		SaveSetting();
	}

	public void OnWaitChange()
	{
		SaveSetting();
	}

	public void OnThickChange()
	{
		SaveSetting();
	}

	public void OnObjDirChange()
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		dir[idx] = objDir.captionText.text;
		SaveSetting();
	}

	public void OnObjColorChange()
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		// color[idx] = objColor.text;
		SaveSetting();
	}

	public void OnObjSpeedChange()
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		speed[idx] = objSpeed.text;
		SaveSetting();
	}

	public void OnTxtBackColor()
	{
		int r = int.Parse(back_R.text);
		int g = int.Parse(back_G.text);
		int b = int.Parse(back_B.text);
		back_color = "00" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
		GetBackColor(backColor);
		SaveSetting();
	}

	public void OnTxtObjCOlor()
	{
		int idx = 0;
		idx = int.Parse(objNo.captionText.text);
		int r = int.Parse(obj_R.text);
		int g = int.Parse(obj_G.text);
		int b = int.Parse(obj_B.text);
		color[idx] = "00" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
		GetObjColor(objColor);
		SaveSetting();
	}

	public void OnTestView()
	{
		// Debug.Log("View Button clicked...");
		// Windows...
		// string anotherURL = System.IO.Path.GetFullPath("FloatingTime.exe");
		// Mac...
		string anotherURL = System.IO.Path.GetFullPath("FloatingTime.app");
		System.Diagnostics.Process anotherPro = new System.Diagnostics.Process();
		anotherPro.StartInfo.FileName = anotherURL;
		anotherPro.Start();
	}

	public void OnSaveAs()
	{
		// var path = EditorUtility.SaveFilePanel(
		// 			"Save json file",
		// 			"",
		// 			"config.json",
		// 			"json");

		var path = StandaloneFileBrowser.SaveFilePanel("保存", "", "config", "json");

		if (!string.IsNullOrEmpty(path))
		{
			int num = 0;
			num = int.Parse(numOfCount.text);
			json[0].name = "NumOfCounter"; json[0].value = numOfCount.text;
			json[1].name = "Depth"; json[1].value = depth.text;
			json[2].name = "Ceiling"; json[2].value = ceiling.text;
			json[3].name = "Background"; json[3].value = back_color;
			json[4].name = "Thickness"; json[4].value = thickness.text;
			json[5].name = "OSWait"; json[5].value = oswait.text;
			for(int i = 0; i < num; i++)
			{
				json[i*3+6].name = "Color" + i.ToString(); json[i*3+6].value = color[i];
				json[i*3+7].name = "Speed" + i.ToString(); json[i*3+7].value = speed[i];
				json[i*3+8].name = "Direction" + i.ToString(); json[i*3+8].value = dir[i];
			}
			string jsonString = "";
			for(int i = 0; i < 6 + num * 3; i++)
			{
				if(i == 0){
					jsonString = JsonUtility.ToJson(json[i]);
				}
				else 
				{
					jsonString += ",\n" + JsonUtility.ToJson(json[i]);
				}
			}
			jsonString = "{\n\"parameters\":[\n" + jsonString + "\n]\n}";
			// Debug.Log(jsonString);
			File.WriteAllText(path, jsonString);
			// EditorUtility.DisplayDialog("通知", "ファイルが保存されました。", "はい");
		}
	}

	public void OnImport()
	{
		string[] path = StandaloneFileBrowser.OpenFilePanel("開く", "", "json", false);
		if (path.Length != 0)
		{
			string importPath = path[0];
			File.WriteAllText(configureFile, File.ReadAllText(importPath, Encoding.UTF8));
			Start();
			OnTestView();
		}
	}

	void SaveSettingData()
	{

	}

    // This is the actual window.
	// void DialogWindow (int windowID)
	// {
	// 	float y = 20;
	// 	GUI.Label(new Rect(5,y, windowRect.width, 20), "ファイルが保存されました。");

	// 	if(GUI.Button(new Rect(5,y, windowRect.width - 10, 20), "OK"))
	// 	{
	// 		Application.LoadLevel (0);
	// 		show = false;
	// 	}
	// }

}
