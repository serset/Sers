package Sers.Core.Module.Message;

import java.util.ArrayList;
import java.util.List;

import Sers.Core.Util.Data.ArraySegment;

public class SersFile {

	   public SersFile() { }

       public SersFile(ArraySegment oriData)
       {
           Unpack(oriData);
       }


       protected  List<ArraySegment> Files;
       
       
       public List<ArraySegment> Files_Get(){    	   
    	   return Files;    	   
       }
       
       
       public void AddFile(ArraySegment file){
    	   Files.add(file);  	   
       }

       
       //region 拆包 与 打包     
       public SersFile Unpack(ArraySegment  oriData)
       {
           Files = UnpackOriData(oriData); 
           return this;
       }



       public List<ArraySegment> Package()
       {
           return PackageArraySegmentByte(Files);
       }
       //endregion

       
       
       
       

       //region static  Package Unpack

       /**
               * 每个文件为 ArraySegmentByte 类型
        * @param files
        * @return
        */
       static List<ArraySegment> PackageArraySegmentByte(List<ArraySegment> files)
       {
    	   ArrayList<ArraySegment> oriData = new ArrayList<>();    	  
    	   
           for(ArraySegment file : files) {  
        	   if(null == file || file.count==0) {
        		   oriData.add(ArraySegment.Int32ToArraySegmentByte(0));
        	   }else {
	               oriData.add(ArraySegment.Int32ToArraySegmentByte(file.count));
	               oriData.add(file);
        	   }
           }
           return oriData;
       }       
       
        


       static List<ArraySegment> UnpackOriData(ArraySegment oriData)
       {
    	   ArrayList<ArraySegment> files = new ArrayList<>();
           int index = 0;
           int fileLen;
        
           while (index < oriData.count)
           {
               fileLen = oriData.ReadInt32(index);
               index += 4;
 
               files.add(oriData.Slice(index, fileLen));
               index += fileLen;               
           }
           return files;
       }

       //endregion
}
