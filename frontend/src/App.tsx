import { useEffect, useState } from "react";
import "./App.css";

interface ResumoVagas {
  total: number;
  candidatar: number;
  analisar: number;
  ignorar: number;
  mediaCompatibilidade: number;
}

function App() {
  const [resumo, setResumo] = useState<ResumoVagas | null>(null);

  useEffect(() => {
    async function carregarResumo() {
      const resposta = await fetch(
        "http://localhost:5245/api/vagas/resumo"
      );

      const dados = await resposta.json();

      setResumo(dados);
    }

    carregarResumo();
  }, []);

  if (!resumo) {
    return <h1>Carregando...</h1>;
  }

return (
    <div className="container">
      <h1>JobHunter AI</h1>

      <div className="cards">
        <div className="card">
          <h2>{resumo.total}</h2>
          <p>Vagas analisadas</p>
        </div>

        <div className="card">
          <h2>{resumo.candidatar}</h2>
          <p>Candidatar</p>
        </div>

        <div className="card">
          <h2>{resumo.analisar}</h2>
          <p>Analisar</p>
        </div>

        <div className="card">
          <h2>{resumo.ignorar}</h2>
          <p>Ignorar</p>
        </div>

        <div className="card">
          <h2>{resumo.mediaCompatibilidade}%</h2>
          <p>Média de compatibilidade</p>
        </div>
      </div>
    </div>
  );
}

export default App;