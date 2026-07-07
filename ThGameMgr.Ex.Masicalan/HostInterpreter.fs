namespace ThGameMgr.Ex.Masicalan

open Masicalan.Core

module HostInterpreter =
    
    /// <summary>
    /// Masicalan スクリプトを実行します．
    /// </summary>
    /// <param name="script">スクリプト</param>
    /// <param name="extVarEnv">Extension 変数環境</param>
    /// <param name="extFunEnv">Extension 関数環境</param>
    let Run (script: string) (extVarEnv: Map<string, Value>) (extFunEnv: Map<string, (string list * Statement)>) =
        try
            Interpreter.RunWithExt script extVarEnv extFunEnv
        with
        | :? System.Reflection.TargetInvocationException as ex -> raise ex.InnerException
        |_ as ex -> raise ex