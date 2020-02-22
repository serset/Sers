package Sers.Core.Module.Reflection;

// https://blog.csdn.net/zp357252539/article/details/72848538 

import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.net.URL;  
import java.net.URLClassLoader;  
import java.util.ArrayList;  
import java.util.Enumeration;  
import java.util.List;  
import java.util.jar.JarEntry;  
import java.util.jar.JarFile;
import java.util.stream.Collectors;

import Sers.Core.Util.Common.CommonHelp;  
  
public class ReflectionHelp {  
  
   
	 public static List<Class> getClasses(String packageName) {  
		 List<String> names= getClassName(packageName, true);  
		  
		 return names.stream().map(name->{
			 Class clazz=null;
			 if(CommonHelp.StringIsNullOrEmpty(name))return clazz;
			 
			 try {
					Class type=Thread.currentThread().getContextClassLoader().loadClass(name);
//					Class type = Class.forName(className);
					return type;
				 
			 }catch(Exception e) {}			
			
			 return clazz;
		 }).filter(clazz->(clazz!=null)).collect(Collectors.toList());
		 
	 
	   }  
	  
  
    /** 
     * 获取某包下（包括该包的所有子包）所有类 
     * @param packageName 包名 
     * @return 类的完整名称 
     */  
    public static List<String> getClassName(String packageName) {  
        return getClassName(packageName, true);  
    }  
  
    /** 
     * 获取某包下所有类 
     * @param packageName 包名 
     * @param childPackage 是否遍历子包 
     * @return 类的完整名称 
     */  
    public static List<String> getClassName(String packageName, boolean childPackage) {  
        List<String> fileNames = new ArrayList<>();  
        ClassLoader loader = Thread.currentThread().getContextClassLoader();  
        String packagePath = packageName.replace(".", "/");  
        try {
        	Enumeration<URL> urls=loader.getResources(packagePath);
        	while(urls.hasMoreElements()) {
        		 URL url = urls.nextElement();  
        	        if (url != null) {  
        	            String type = url.getProtocol();  
        	            if (type.equals("file")) {  
        	                getClassNameByFile(packageName,url.getPath(), fileNames, childPackage);  
        	            } else if (type.equals("jar")) {  
        	                getClassNameByJar(url.getPath(), childPackage,fileNames);  
        	            }  
        	        } else {  
        	            getClassNameByJars(((URLClassLoader) loader).getURLs(), packagePath, childPackage,fileNames);  
        	        }  
        	}
			 
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

        return fileNames;  
    }  
  
    /** 
     * 从项目文件获取某包下所有类 
     * @param filePath 文件路径 
     * @param className 类名集合 
     * @param childPackage 是否遍历子包 
     * @return 类的完整名称 
     */  
    private static void getClassNameByFile(String packageName,String packagePath, List<String> myClassName, boolean childPackage) {  
    
        File[] childFiles = new File(packagePath).listFiles();  
        for (File file : childFiles) {  
        	String name=file.getName();
        	
            if (file.isDirectory()) {  
                if (childPackage) {  
                	//递归调用一直到文件目录最后一级
                	
                    String subPackagePath =  packagePath+ File.pathSeparatorChar +name; 
                    
                    String subPackageName =  packageName + "." + name;
                    
                	getClassNameByFile(subPackageName,subPackagePath, myClassName, childPackage);  
                }  
            } else if (file.isFile()){
             
                if (name.endsWith(".class")) {  
                	 String className = packageName + "." +  name.substring(0,name.lastIndexOf("."));
                    myClassName.add(className);  
                }  
            }  
        }   
    }  
     
  
    /** 
     * 从jar获取某包下所有类 
     * @param jarPath jar文件路径 
     * @param childPackage 是否遍历子包 
     * @return 类的完整名称 
     */  
    private static void getClassNameByJar(String jarPath, boolean childPackage,List<String> myClassName) {  
        
        String[] jarInfo = jarPath.split("!");  
        String jarFilePath = jarInfo[0].substring(jarInfo[0].indexOf("/"));  
        String packagePath = jarInfo[1].substring(1);  
        try {  
            JarFile jarFile = new JarFile(jarFilePath);  
            Enumeration<JarEntry> entrys = jarFile.entries();  
            while (entrys.hasMoreElements()) {  
                JarEntry jarEntry = entrys.nextElement();  
                
                String entryName = jarEntry.getName();            
                if (entryName.endsWith(".class")) {  
                    if (childPackage) {  
                        if (entryName.startsWith(packagePath)) {  
                            entryName = entryName.replace("/", ".").substring(0, entryName.lastIndexOf("."));  
                            myClassName.add(entryName);  
                        }  
                    } else {  
                        int index = entryName.lastIndexOf("/");  
                        String myPackagePath;  
                        if (index != -1) {  
                            myPackagePath = entryName.substring(0, index);  
                        } else {  
                            myPackagePath = entryName;  
                        }  
                        if (myPackagePath.equals(packagePath)) {  
                            entryName = entryName.replace("/", ".").substring(0, entryName.lastIndexOf("."));  
                            myClassName.add(entryName);  
                        }  
                    }  
                }  
            }  
        } catch (Exception e) {  
            e.printStackTrace();  
        }  
  
    }  
  
    /** 
     * 从所有jar中搜索该包，并获取该包下所有类 
     * @param urls URL集合 
     * @param packagePath 包路径 
     * @param childPackage 是否遍历子包 
     * @return 类的完整名称 
     */  
    private static void getClassNameByJars(URL[] urls, String packagePath, boolean childPackage,List<String> myClassName) {  
     
        if (urls != null) {  
            for (int i = 0; i < urls.length; i++) {  
                URL url = urls[i];  
                String urlPath = url.getPath();  
                // 不必搜索classes文件夹  
                if (urlPath.endsWith("classes/")) {  
                    continue;  
                }  
                String jarPath = urlPath + "!/" + packagePath;  
                 getClassNameByJar(jarPath, childPackage,myClassName );  
            }  
        }  
    
    }  
}  