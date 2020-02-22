package Test;

import java.util.List;

import Sers.Core.Module.Reflection.ReflectionHelp;

public class ReflectionTest {

	 public static void Test() throws Exception {  
	        String packageName = "Sers";  
	        // List<String> classNames = getClassName(packageName);  
	        List<String> classNames = ReflectionHelp.getClassName("Sers", true);  
//	        if (classNames != null) {  
//	            for (String className : classNames) {  
//	                System.out.println(className);  
//	            }  
//	        }  
	        
	        classNames = ReflectionHelp.getClassName("com.google", true);  
	        if (classNames != null) {  
	            for (String className : classNames) {  
	                System.out.println(className);  
	            }  
	        }  
	    }  

}
