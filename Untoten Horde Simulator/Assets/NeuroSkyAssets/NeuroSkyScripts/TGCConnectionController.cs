using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class TGCConnectionController : MonoBehaviour {
	private TcpClient client; 
  	private Stream stream;
  	private byte[] buffer;
	
	public delegate void UpdateIntValueDelegate(int value);
	public delegate void UpdateFloatValueDelegate(float value);
	
	public event UpdateIntValueDelegate UpdatePoorSignalEvent;
	public event UpdateIntValueDelegate UpdateAttentionEvent;
	public event UpdateIntValueDelegate UpdateMeditationEvent;
	public event UpdateIntValueDelegate UpdateRawdataEvent;
	public event UpdateIntValueDelegate UpdateBlinkEvent;
	
	public event UpdateIntValueDelegate UpdateDeltaEvent;
	public event UpdateIntValueDelegate UpdateThetaEvent;
	public event UpdateIntValueDelegate UpdateLowAlphaEvent;
	public event UpdateIntValueDelegate UpdateHighAlphaEvent;
	public event UpdateIntValueDelegate UpdateLowBetaEvent;
	public event UpdateIntValueDelegate UpdateHighBetaEvent;
	public event UpdateIntValueDelegate UpdateLowGammaEvent;
	public event UpdateIntValueDelegate UpdateHighGammaEvent;

	void Start () {
		Connect();
	}
	
	public void Disconnect(){
		if(IsInvoking("ParseData")){
			CancelInvoke("ParseData");
			stream.Close();
		}
	}
	
	public void Connect(){
		if(!IsInvoking("ParseData")){
			
			client = new TcpClient("127.0.0.1", 13854);	
		    stream = client.GetStream();
		    buffer = new byte[1024];
		    byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
		    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
			
			InvokeRepeating("ParseData",0.0f,1.1f);
		}
	}
	
	void ParseData(){
	    if(stream.CanRead){
	      try { 
	        int bytesRead = stream.Read(buffer, 0, buffer.Length);
	
	        string[] packets = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split('\r');
	
	        foreach(string packet in packets){
	          if(packet.Length == 0)
	            continue;
	
	          IDictionary primary = (IDictionary)JsonConvert.Import(typeof(IDictionary), packet);
	
	          if(primary.Contains("poorSignalLevel")){
						
				if(UpdatePoorSignalEvent != null){
				   UpdatePoorSignalEvent(int.Parse(primary["poorSignalLevel"].ToString()));
				   }
						
	            if(primary.Contains("eSense")){
	              IDictionary eSense = (IDictionary)primary["eSense"];
				  if(UpdateAttentionEvent != null){
					 UpdateAttentionEvent(int.Parse(eSense["attention"].ToString()));
				   }		
				  if(UpdateMeditationEvent != null){
					 UpdateMeditationEvent(int.Parse(eSense["meditation"].ToString()));
				   }
	            }
	
	            if(primary.Contains("eegPower")){
	              IDictionary eegPowers = (IDictionary)primary["eegPower"];
								
				  if(UpdateDeltaEvent != null){
								UpdateDeltaEvent(int.Parse(eegPowers["delta"].ToString()));			
					}
				  if(UpdateThetaEvent != null){
								UpdateThetaEvent(int.Parse(eegPowers["theta"].ToString()));			
					}
				  if(UpdateLowAlphaEvent != null){
								UpdateLowAlphaEvent(int.Parse(eegPowers["lowAlpha"].ToString()));
					}
				  if(UpdateHighAlphaEvent != null){
								UpdateHighAlphaEvent(int.Parse(eegPowers["highAlpha"].ToString()));
					}
				  if(UpdateLowBetaEvent != null){
								UpdateLowBetaEvent(int.Parse(eegPowers["lowBeta"].ToString()));
					}
				  if(UpdateHighBetaEvent != null){
								UpdateHighBetaEvent(int.Parse(eegPowers["highBeta"].ToString()));
					}
				  if(UpdateLowGammaEvent != null){
								UpdateLowGammaEvent(int.Parse(eegPowers["lowGamma"].ToString()));			
					}
				  if(UpdateHighGammaEvent != null){
								UpdateHighGammaEvent(int.Parse(eegPowers["highGamma"].ToString()));
					}
	            }
	          }
	          else if(primary.Contains("rawEeg") && UpdateRawdataEvent != null){
					  UpdateRawdataEvent(int.Parse(primary["rawEeg"].ToString()));
	          }
	          else if(primary.Contains("blinkStrength") && UpdateBlinkEvent != null){
					  UpdateBlinkEvent(int.Parse(primary["blinkStrength"].ToString()));
	          }
	        }
	      }
	      catch(IOException e){ Debug.Log("IOException " + e); }
			catch(System.Exception ){/*Debug.Log("Exception " + e);*/ }
	    }		
		
	}// end ParseData
	
	void OnApplicationQuit(){
		Disconnect();
	}

	
}
