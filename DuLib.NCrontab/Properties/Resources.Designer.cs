﻿//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Du.Properties {
    using System;
    
    
    /// <summary>
    ///   지역화된 문자열 등을 찾기 위한 강력한 형식의 리소스 클래스입니다.
    /// </summary>
    // 이 클래스는 ResGen 또는 Visual Studio와 같은 도구를 통해 StronglyTypedResourceBuilder
    // 클래스에서 자동으로 생성되었습니다.
    // 멤버를 추가하거나 제거하려면 .ResX 파일을 편집한 다음 /str 옵션을 사용하여 ResGen을
    // 다시 실행하거나 VS 프로젝트를 다시 빌드하십시오.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   이 클래스에서 사용하는 캐시된 ResourceManager 인스턴스를 반환합니다.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Du.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   이 강력한 형식의 리소스 클래스를 사용하여 모든 리소스 조회에 대해 현재 스레드의 CurrentUICulture 속성을
        ///   재정의합니다.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   사월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string April {
            get {
                return ResourceManager.GetString("April", resourceCulture);
            }
        }
        
        /// <summary>
        ///   팔월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string August {
            get {
                return ResourceManager.GetString("August", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Crontab에 오류예요!과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string CrontabError {
            get {
                return ResourceManager.GetString("CrontabError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   십이월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string December {
            get {
                return ResourceManager.GetString("December", resourceCulture);
            }
        }
        
        /// <summary>
        ///   이미 달리고 있어요!과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionAlreadyRun {
            get {
                return ResourceManager.GetString("ExceptionAlreadyRun", resourceCulture);
            }
        }
        
        /// <summary>
        ///   필드에 빈칸이 있으면 안되요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionEmptyField {
            get {
                return ResourceManager.GetString("ExceptionEmptyField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &apos;{0}&apos;: 필드 [{1}]의 알맞는 표현이 아니예요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionInvalidFieldExpression {
            get {
                return ResourceManager.GetString("ExceptionInvalidFieldExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &apos;{0}&apos;: 필드 [{3}]의 알맞는 표현이 아니에요. 반드시 {1}과 {2} 사이에 있어야 해요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionInvalidFieldExpressionParse {
            get {
                return ResourceManager.GetString("ExceptionInvalidFieldExpressionParse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   필드 종류가 틀렸어요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionInvalidFieldKind {
            get {
                return ResourceManager.GetString("ExceptionInvalidFieldKind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &apos;{0}&apos; 표현은 잘못된 구성이예요. 반드시 {1}로 구성해야 해요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionInvalidScheduleExpression {
            get {
                return ResourceManager.GetString("ExceptionInvalidScheduleExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}값은 필드 [{1}]의 최대 허용치 보다 커요. 값은 {2}와 {3}사이로 해주세요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionKindValueOverflow {
            get {
                return ResourceManager.GetString("ExceptionKindValueOverflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}값은 필드 [{1}]의 최소 허용치 보다 작아요. 값은 {2}와 {3}사이로 해주세요과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionKindValueUnderflow {
            get {
                return ResourceManager.GetString("ExceptionKindValueUnderflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &apos;{0}&apos;: 적당한 이름이 아니예요. 다음 중 하나로 고치세요: {1}과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string ExceptionNotKnowFollow {
            get {
                return ResourceManager.GetString("ExceptionNotKnowFollow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   이월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string February {
            get {
                return ResourceManager.GetString("February", resourceCulture);
            }
        }
        
        /// <summary>
        ///   5개 항목으로 된 스케줄은 분-시-일-월-요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string FiveComponentDesc {
            get {
                return ResourceManager.GetString("FiveComponentDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   금요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Friday {
            get {
                return ResourceManager.GetString("Friday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   일월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string January {
            get {
                return ResourceManager.GetString("January", resourceCulture);
            }
        }
        
        /// <summary>
        ///   칠월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string July {
            get {
                return ResourceManager.GetString("July", resourceCulture);
            }
        }
        
        /// <summary>
        ///   유월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string June {
            get {
                return ResourceManager.GetString("June", resourceCulture);
            }
        }
        
        /// <summary>
        ///   삼월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string March {
            get {
                return ResourceManager.GetString("March", resourceCulture);
            }
        }
        
        /// <summary>
        ///   오월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string May {
            get {
                return ResourceManager.GetString("May", resourceCulture);
            }
        }
        
        /// <summary>
        ///   월요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Monday {
            get {
                return ResourceManager.GetString("Monday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   십일월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string November {
            get {
                return ResourceManager.GetString("November", resourceCulture);
            }
        }
        
        /// <summary>
        ///   시월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string October {
            get {
                return ResourceManager.GetString("October", resourceCulture);
            }
        }
        
        /// <summary>
        ///   토요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Saturday {
            get {
                return ResourceManager.GetString("Saturday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   구월과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string September {
            get {
                return ResourceManager.GetString("September", resourceCulture);
            }
        }
        
        /// <summary>
        ///   6개 항목으로 된 스케줄은 초-분-시-일-월-요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string SixComponentDesc {
            get {
                return ResourceManager.GetString("SixComponentDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   일요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Sunday {
            get {
                return ResourceManager.GetString("Sunday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   목요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Thursday {
            get {
                return ResourceManager.GetString("Thursday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   화요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Tuesday {
            get {
                return ResourceManager.GetString("Tuesday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   수요일과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string Wednesday {
            get {
                return ResourceManager.GetString("Wednesday", resourceCulture);
            }
        }
        
        /// <summary>
        ///   DuLib.NCrontab과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        internal static string WhatsNCrontab {
            get {
                return ResourceManager.GetString("WhatsNCrontab", resourceCulture);
            }
        }
    }
}
