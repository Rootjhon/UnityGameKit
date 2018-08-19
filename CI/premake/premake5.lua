--[[Util Start--]]
function CurFileDir()
      local path = debug.getinfo(1, "S").source
      path = string.sub(path, 2, -1)
      path = string.match(path, "^.*/")
      return path
end
--[[Util End--]]


--[[Premake Start--]]
function CreateNewPrj(sPrjName,sPath)
      project (sPrjName)
      kind ("SharedLib")
      uuid (os.uuid(sPrjName))
      location (sPath)
      files(sPath .. "/**.cs")
end
function UnityDependencies(sUnityVersion)
      local UnityDeps = {
            "UnityEditor",
            "UnityEngine",
      }

      for _,v in ipairs(UnityDeps) do
            links{ string.format("../../Dependencies/%s/",v) }
      end
end
--[[Premake End--]]

workspace "UnityGameKit"
      configurations { "Android", "IOS"}
      language "C#"         
      optimize "On"
      clr "Unsafe"
      location "../../"
      targetdir "../../bin"
      dotnetframework  "3.5"
      links { "System","System.Data","System.Core"}

UnityDependencies("Unity5.5.2p1")

CreateNewPrj("BaseKit","../../BaseKit" )

      --unity 各个平台宏设置
      -- filter {"configurations:Android"}
      -- filter {"configurations:IOS"}
      -- filter {"configurations:Editor_Win"}

-- project "BaseKit"
--       kind "SharedLib"
--       CreateUUID()
--       location "../../BaseKit"      
--       files {"../../BaseKit/Core/**.cs"}
--       files { "../../BaseKit/Properties/AssemblyInfo.cs" } 

