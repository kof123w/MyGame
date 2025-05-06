Shader "ShaderFrameWork/Diffuse"
{
    Properties
    {
        _BaseMap("BaseMap",2d) = "white"{}
        _BaseColor("BaseColor",Color) = (1,1,1,1)
        _BaseColorInv("BaseColorInv",Float) = 1.0
        [Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping",Float) = 0
        //剔除a值
        _Cutoff("Cut Off",Float) = 0.5
        //剔除模式选择
        [Enum(off,0,Front,1,Back,2)]_Cull("Cull Mode",Float) = 2.0
    }
    
    SubShader
    {
        LOD 200
        Name "ShaderFW-ForwardLit"
        
        Cull [_Cull]
        
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
           float4 _BaseMap_ST;
           float4 _BaseColor;
           float _BaseColorInv;
           float _Cutoff;
        CBUFFER_END
        ENDHLSL

        Pass
        {
             Name "ShaderFW-DiffusePass"
            Tags
            {
                "LightMode"="UniversalForward"
            }
            
            HLSLPROGRAM

            #pragma vertex UnLitVert
            #pragma fragment UnLitFrag
            #pragma shader_feature _ALPHATEST_ON   //透明剔除开关
            TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
            struct Attribute
            {
                float4 positionOS    : POSITION;
                float4 normalOS      : NORMAL;
                float2 uv            : TEXCOORD0;
                float2 lightMapUV    : TEXCOORD1;
                float4 color         : COLOR;
            };

            struct Varying
            {
                float4 postionCS     : SV_POSITION;
                float2 uv            : TEXCOORD0;
               //  DECLARE_LIGHTMAP_OR_SH(lightMapUV,vertexSH,1);
                float3 normalsWS     : TEXCOORD2;
                float3 positionWS    :TEXCOORD3;
                float4 color         : COLOR;
            };

            Varying UnLitVert(Attribute i)
            {
                Varying o;
                VertexPositionInputs inputs = GetVertexPositionInputs(i.positionOS);
                o.postionCS = inputs.positionCS;
                o.positionWS = inputs.positionWS;
                VertexNormalInputs normals_inputs = GetVertexNormalInputs(i.normalOS.xyz);
                o.normalsWS = normals_inputs.normalWS;
                
              //  OUTPUT_LIGHTMAP_UV(i.lightMapUV,unity_LightmapST,o.normalsWS);
              //  OUTPUT_SH(o.normalsWS,o.vertextSH);
                o.uv = TRANSFORM_TEX(i.uv,_BaseMap);
                o.color = i.color;
                return o;
            }

            half4 UnLitFrag(Varying i) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv);
                #ifdef _ALPHATEST_ON
                clip(baseMap.a - _Cutoff);
                #endif
                
                return baseMap * i.color * _BaseColor * _BaseColorInv;
            }
            
            ENDHLSL
        }


        Pass
        {
            Name "ShaderFw-Unlit-ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            ZWrite On
            ZTest LEqual
            
            ColorMask 0
            Cull [_Cull]
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex;
            #pragma fragment ShadowPassFragment;
            //材质相关
            #pragma shader_feature _ALPHATEST_ON   //透明剔除开关
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBED0_CHANGEL_A
            #pragma multi_compile_instancing  //GPU实例化

            //阴影相关
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW   //支持局部光
          
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            
            ENDHLSL
        }

       // UsePass "Universal Render Pipeline/Lit/ShadowCaster"

        Pass
       {
          Name "DepthOnly"
          Tags{"LightMode"="DepthOnly"}
          
          ZWrite On
          ZTest LEqual
          
          ColorMask 0
          
          HLSLPROGRAM
            //材质相关
            #pragma shader_feature _ALPHATEST_ON   //透明剔除开关
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBED0_CHANGEL_A
            #pragma multi_compile_instancing  //GPU实例化
          
          #pragma vertex DepthOnlyVertex
          #pragma fragment DepthOnlyFragment

          #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
          #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
          #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
          ENDHLSL
       }

       //MSAA,etc...
       Pass
       {
           Name "DepthNormals"
          Tags{"LightMode"="DepthNormals"}
          
          ZWrite On
          ZTest LEqual
          
          ColorMask 0
          
          HLSLPROGRAM
            //材质相关
            #pragma shader_feature_local _NORMAL_MAP
            #pragma shader_feature _ALPHATEST_ON   //透明剔除开关
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBED0_CHANGEL_A
            #pragma multi_compile_instancing  //GPU实例化
          
          #pragma vertex DepthNormalsVertex
          #pragma fragment DepthNormalsFragment

          #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
          #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
          #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
          ENDHLSL
       }
    } 
     FallBack "Hidden/Universal Render Pipeline/FallbackError"  //填写故障情况下最保守shader的pass路径
}