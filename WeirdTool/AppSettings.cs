﻿namespace WeirdTool
{
    public static class AppSettings
    {
        private static IConfiguration? _configuration;
        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetConfig(string key)
        {
            return _configuration?[key]??"";
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string GetConfig(params string[] sections)
        {
            try
            {
                if (sections.Length != 0)
                {
                    return _configuration?[string.Join(":", sections)]??"";
                }
            }
            catch (Exception) { }

            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> GetConfig<T>(params string[] sections)
        {
            List<T> list = new();
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            _configuration?.Bind(string.Join(":", sections), list);
            return list;
        }
    }
}
