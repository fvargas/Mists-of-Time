using UnityEngine;
using System.Collections;

public class Utils {
	public static float FLOAT_COMP_PRECISION = 0.001f;
	public static bool isEqual(float a,float b){
		return (Mathf.Abs(a - b) <= FLOAT_COMP_PRECISION);
	}
	public static string toString(int [,,] m){
		string res = "";
		for(int i=0;i<m.GetLength(1);i++){
			for(int j=0;j<m.GetLength(0);j++){
				for(int k=0;k<m.GetLength(2);k++){
					res += m[j,i,k]+",";
				}
				res += "\n";
			}
			res += "\n";
		}
		return res;
	}
}
