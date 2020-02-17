package Sers.Core.Util.Common;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.StringReader;
import java.nio.file.Files;
import java.nio.file.Path;
import java.text.SimpleDateFormat;
import java.util.Date;

import Sers.Core.Module.Serialization.Serialization;
import Sers.Core.Util.Data.ArraySegment;

public class CommonHelp {

	static final SnowflakeIdWorker idWorker = new SnowflakeIdWorker();
  
    public  static long NewGuidLong()
    {
    	return idWorker.nextId();   
    }

    public static String NewGuid()
    {
        return ""+NewGuidLong();
    }

    // return demo: /root/ws/Gateway/Debug/Gateway
    public static String GetBaseDirectory()
    {
        return System.getProperty("user.dir");
    }

    //path demo:    "Data","Demo.json"
    public static String GetAbsPathByRealativePath(String ...  paths){
        String absPath=GetBaseDirectory();
        for ( String path :paths) {
            absPath+= File.separatorChar +path;
        }
        return absPath;
    }

    //path demo:    Data/Demo.json
    public static String GetAbsPathByRealativePath(String  path){
        return GetBaseDirectory()+  File.separatorChar +path;
    }


    /**
     * "yyyy-MM-dd HH:mm:ss"
     * @param date
     * @param format
     * @return
     */
    public static String DateToString(Date date, String format){
        return   new SimpleDateFormat(format).format(date);
    }

    /**
     *
     * @param value
     * @return
     */
    public static boolean StringIsNullOrEmpty(String value){
        return value==null || "".equals(value);
    }

    public static final String NewLine="\r\n";
    
    
    //region (x.3)txt File    
    public static String File_ReadAllText(String filePath) {
    	try {
    		
            File txtFile = new File(filePath);
            if(!txtFile.exists()){
           	  return null;
            }
            
            int length=(int)txtFile.length();
            
            if(length<=0)return null;
           
            try (FileInputStream in =new FileInputStream(txtFile)             
            ){           	
            	
            	byte[] data=new byte[length];
            	int len=in.read(data); 
            	return Serialization.Instance.bytesToString(new ArraySegment(data));
            }
        } catch (IOException e) {
//            e.printStackTrace();
        }
    	return null;
    }
    
    public synchronized static void File_AppendText(String filePath,String content) {
    	 try {
             File txtFile = new File(filePath);
             if(!txtFile.exists()){
            	 txtFile.createNewFile();
             }

             try (FileWriter writer = new FileWriter(txtFile,true);
                  BufferedWriter out = new BufferedWriter(writer)
             ){
                 out.write(content);
                 out.flush(); // 把缓存区内容压入文件
             }
         } catch (IOException e) {
//             e.printStackTrace();
         }
    }
    
    //endregion
    
    
    public interface ICallWhenNotNull_Action<T1,T2>{    	
    	public T2 invoke(T1 obj);    	
    }
    
    public static <T1,T2> T2 CallWhenNotNull(T1 obj,ICallWhenNotNull_Action<T1,T2> action){
    	if(null==obj) return null;    	
    	return action.invoke(obj);
    }
    
    
    
}
