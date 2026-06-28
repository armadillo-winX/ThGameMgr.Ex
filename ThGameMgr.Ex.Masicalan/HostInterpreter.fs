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
        Interpreter.RunWithExt script extVarEnv extFunEnv