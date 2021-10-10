using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CustomShaderEditor : ShaderGUI
{
    static bool isShowDisplayName = true;

    // Start is called before the first frame update
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {

    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        materialEditor.SetDefaultGUIWidths();
        Shader shader = (materialEditor.target as Material).shader;

        isShowDisplayName = EditorGUILayout.Toggle("显示名字(变量名)", isShowDisplayName);

        for (int i = 0; i < props.Length; i++)
        {
            string[] attributes = shader.GetPropertyAttributes(i);

            //如果没有标签则默认输出
            if (attributes.Length == 0)
            {
                materialEditor.ShaderProperty(props[i], PropName(props[i]));
                continue;
            }

            //遍历每个字段的标签
            bool isSpecial = false;

            foreach (var flag in attributes)
            {
                //开关
                if (flag.StartsWith("follow"))
                {
                    isSpecial = true;
                    Match match = Regex.Match(flag, @"\w+\s*\((\w+)[,]\s*([0-9])\)");

                    if (match.Success)
                    {
                        MaterialProperty materialProperty = FindProperty(match.Groups[1].Value, props, false);

                        if (materialProperty != null)
                        {
                            if (materialProperty.floatValue >= int.Parse(match.Groups[2].Value))
                            {
                                materialEditor.ShaderProperty(props[i], PropName(props[i]), 1);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("CustomShaderEditor:follow:没有找到");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("CustomShaderEditor:follow:请书写正确的格式 比如：follow(_Test02, 1)");
                    }
                }

                //下拉框
                if (flag.StartsWith("popup"))
                {
                    isSpecial = true;

                    //获取括号中的内容
                    Match match = Regex.Match(flag, @"\w+\((\S+)\)");

                    if (match.Success)
                    {
                        #region 按","读取字符中的信息
                        string str = match.Groups[1].Value;
                        List<string> strs = new List<string>();
                        List<int> nums = new List<int>();
                        string temp = null;

                        for (int j = 0; j < str.Length; j++)
                        {
                            if (str[j] != ',')
                            {
                                temp += str[j];

                            }
                            else
                            {
                                if (strs.Count <= nums.Count)
                                {
                                    strs.Add(temp);
                                }
                                else
                                {
                                    nums.Add(int.Parse(temp));
                                }

                                temp = null;
                                continue;
                            }
                        }

                        if(temp != null)
                        {
                            nums.Add(int.Parse(temp));
                        }

                        #endregion

                        if (strs.Count != 0)
                        {
                            int mode = (int)props[i].floatValue;

                            if (nums.Contains(mode))
                            {
                                mode = nums.IndexOf(mode);
                            }
                            else
                            {
                                Debug.LogWarning("CustomShaderEditor:popup:索引数不匹配");
                            }

                            mode = EditorGUILayout.Popup(PropName(props[i]), mode, strs.ToArray());

                            props[i].floatValue = nums[mode];

                            
                        }
                        else
                        {
                            Debug.LogWarning("CustomShaderEditor:popup:请在括号中写点东西 比如：popup(AlphaBlend,10,Additive,1)");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("CustomShaderEditor:popup:请不要在括号中出现空格");
                    }
                }

                //透明通道
                
            }

            if (!isSpecial)
            {
                materialEditor.ShaderProperty(props[i], PropName(props[i]));
            }
        }
    }

    string PropName(MaterialProperty materialProperty)
    {
        return isShowDisplayName ? materialProperty.displayName : materialProperty.name;
    }
}




