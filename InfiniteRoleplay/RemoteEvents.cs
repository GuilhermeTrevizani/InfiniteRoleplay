using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace InfiniteRoleplay
{
    public class RemoteEvents : Script
    {
        [RemoteEvent("playersOnline")]
        public void EVENT_playersOnline(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var personagens = Global.PersonagensOnline.Where(x => x.ID > 0).OrderBy(x => x.ID == p.ID ? 0 : 1).ThenBy(x => x.ID).Select(x => new { id = x.ID, nome = x.Nome, ping = x.Player.Ping }).ToList();
            NAPI.ClientEvent.TriggerClientEvent(player, "playersOnline", personagens);
        }

        [RemoteEvent("registrarUsuario")]
        public void EVENT_registrarUsuario(Client player, string usuario, string email, string senha, string senha2)
        {
            var p = Functions.ObterPersonagem(player);
            if (p != null)
                return;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(senha2))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            if (usuario.Length > 25)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Usuário não pode ter mais que 25 caracteres!");
                return;
            }

            if (email.Length > 100)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Email não pode ter mais que 100 caracteres!");
                return;
            }

            if (senha != senha2)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Senhas não estão iguais!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var uSocial = context.Usuarios.FirstOrDefault(x => x.SocialClubRegistro == player.SocialClubName);
                if (uSocial != null)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", uSocial.Nome, $"Você já possui o usuário {uSocial.Nome}!");
                    return;
                }

                if (context.Usuarios.Any(x => x.Nome == usuario))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, $"Usuário {usuario} já existe!");
                    return;
                }

                if (context.Usuarios.Any(x => x.Email == email))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, $"Email {email} já está sendo utilizado!");
                    return;
                }

                var user = new Usuario()
                {
                    Nome = usuario,
                    Email = email,
                    Senha = Functions.Criptografar(senha),
                    SocialClubRegistro = player.SocialClubName,
                    SocialClubUltimoAcesso = player.SocialClubName,
                    Serial = player.Serial,
                    IPRegistro = player.Address,
                    IPUltimoAcesso = player.Address,
                };
                context.Usuarios.Add(user);
                context.SaveChanges();
            }

            EVENT_entrarUsuario(player, usuario, senha);
        }

        [RemoteEvent("entrarUsuario")]
        public void EVENT_entrarUsuario(Client player, string usuario, string senha)
        {
            var p = Functions.ObterPersonagem(player);
            if (p != null)
                return;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", usuario, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            var senhaCriptografada = Functions.Criptografar(senha);
            using (var context = new RoleplayContext())
            {
                var user = context.Usuarios.FirstOrDefault(x => x.Nome == usuario && x.Senha == senhaCriptografada);
                if (user == null)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", usuario, "Usuário ou senha inválidos!");
                    return;
                }

                var banimento = context.Banimentos.FirstOrDefault(x => x.Usuario == user.Codigo);
                if (!Functions.VerificarBanimento(player, banimento))
                    return;

                user.DataUltimoAcesso = DateTime.Now;
                user.IPUltimoAcesso = player.Address;
                user.SocialClubUltimoAcesso = player.SocialClubName;
                context.Usuarios.Update(user);
                context.SaveChanges();

                Global.PersonagensOnline.Add(new Personagem()
                {
                    UsuarioBD = user,
                });

                EVENT_voltarSelecionarPersonagem(player);
            }
        }

        [RemoteEvent("criarPersonagem")]
        public void EVENT_criarPersonagem(Client player, string nome, string sobrenome, string sexo, string dataNascimento, string skin)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.ID > 0)
                return;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(sexo)
                || string.IsNullOrWhiteSpace(dataNascimento) || string.IsNullOrWhiteSpace(skin))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome);
            sobrenome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sobrenome);
            var nomeCompleto = $"{nome} {sobrenome}";
            if (nomeCompleto.Length > 25)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Nome do personagem não pode possuir mais que 25 caracteres!");
                return;
            }

            sexo = sexo.ToUpper();
            if (sexo != "F" && sexo != "M")
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Sexo deve ser M ou F!");
                return;
            }

            DateTime.TryParse(dataNascimento, out DateTime dtNascimento);
            if (dtNascimento == DateTime.MinValue)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Data de Nascimento não foi informada corretamente!");
                return;
            }

            var dif = DateTime.Now.Date - dtNascimento;
            if (dif.Days / 365 < 18)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Personagem precisa ter 18 anos ou mais!");
                return;
            }

            var pedHash = NAPI.Util.PedNameToModel(skin);
            if (pedHash == 0)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Skin não existe!");
                return;
            }

            if (!Global.Skins.Any(x => x.Nome == pedHash.ToString() && !x.IsBloqueada && x.TipoFaccao == null && x.Sexo == sexo))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, "Skin bloqueada ou indisponível para o sexo do seu personagem!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Personagens.Any(x => x.Nome == nomeCompleto))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, skin, $"Personagem {nomeCompleto} já existe!");
                    return;
                }

                var personagem = new Personagem()
                {
                    Nome = nomeCompleto,
                    Usuario = p.UsuarioBD.Codigo,
                    Sexo = sexo,
                    DataNascimento = dtNascimento,
                    SocialClubRegistro = player.SocialClubName,
                    SocialClubUltimoAcesso = player.SocialClubName,
                    IPRegistro = player.Address,
                    IPUltimoAcesso = player.Address,
                    ID = Functions.ObterNovoID(),
                    Skin = pedHash.ToString(),
                };

                var ultimoPersonagem = context.Personagens.LastOrDefault(x => x.Usuario == p.UsuarioBD.Codigo);
                if (ultimoPersonagem != null)
                {
                    personagem.Banco += ultimoPersonagem.Dinheiro + ultimoPersonagem.Banco;

                    foreach (var prop in ultimoPersonagem.Propriedades)
                    {
                        prop.Personagem = 0;
                        personagem.Banco += prop.Valor;
                        prop.CriarIdentificador();
                        context.Propriedades.Update(prop);
                    }

                    var veiculos = context.Veiculos.Where(x => x.Personagem == ultimoPersonagem.Codigo).ToList();
                    foreach(var veh in veiculos)
                    {
                        var preco = Global.Precos.FirstOrDefault(x => x.Tipo == (int)TipoPreco.CarrosMotos && x.Nome == veh.Modelo);
                        personagem.Banco += preco?.Valor ?? 0; 
                        context.Veiculos.Remove(veh);
                    }

                    personagem.Banco = Convert.ToInt32(personagem.Banco * (p.UsuarioBD.PossuiNamechange ? 0.75 : 0.25));
                }

                context.Personagens.Add(personagem);

                p.UsuarioBD.PossuiNamechange = false;
                context.Usuarios.Update(p.UsuarioBD);

                context.SaveChanges();

                var user = p.UsuarioBD;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = personagem;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(personagem)].UsuarioBD = user;

                Functions.LogarPersonagem(player, personagem);
            }
        }

        [RemoteEvent("selecionarPersonagem")]
        public void EVENT_selecionarPersonagem(Client player, int id)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null || p?.ID > 0)
                return;

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == id && x.Usuario == p.UsuarioBD.Codigo && x.DataMorte == null);
                if (personagem == null)
                {
                    EVENT_voltarSelecionarPersonagem(player);
                    return;
                }

                personagem.DataUltimoAcesso = DateTime.Now;
                personagem.IPUltimoAcesso = player.Address;
                personagem.SocialClubUltimoAcesso = player.SocialClubName;
                personagem.ID = Functions.ObterNovoID();
                personagem.Online = true;
                context.Personagens.Update(personagem);
                context.SaveChanges();

                var user = p.UsuarioBD;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = personagem;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(personagem)].UsuarioBD = user;

                Functions.LogarPersonagem(player, personagem);
            }
        }

        [RemoteEvent("voltarSelecionarPersonagem")]
        public void EVENT_voltarSelecionarPersonagem(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null || p?.ID > 0)
                return;

            using (var context = new RoleplayContext())
            {
                var personagens = context.Personagens.Where(x => x.Usuario == p.UsuarioBD.Codigo && x.DataMorte == null).OrderBy(x => x.Codigo).Select(x => new { id = x.Codigo, nome = x.Nome }).ToList();
                if (personagens.Count == 1)
                {
                    EVENT_selecionarPersonagem(player, personagens.FirstOrDefault().id);
                    return;
                }

                NAPI.ClientEvent.TriggerClientEvent(player, "selecionarPersonagem", personagens);
            }
        }

        [RemoteEvent("comprarVeiculo")]
        public void EVENT_comprarVeiculo(Client player, int tipo, string veiculo, int r1, int g1, int b1, int r2, int g2, int b2)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (string.IsNullOrWhiteSpace(veiculo))
            {
                Functions.ComprarVeiculo(player, tipo, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == tipo && x.Nome.ToLower() == veiculo.ToLower());
            if (preco == null)
            {
                Functions.ComprarVeiculo(player, tipo, "Veículo não está disponível para compra!");
                return;
            }

            if (p.Dinheiro < preco.Valor)
            {
                Functions.ComprarVeiculo(player, tipo, "Você não possui dinheiro suficiente!");
                return;
            }

            var concessionaria = Global.Concessionarias.FirstOrDefault(x => x.Tipo == (TipoPreco)tipo);

            var veh = new Veiculo()
            {
                Personagem = p.Codigo,
                Cor1R = r1,
                Cor1G = g1,
                Cor1B = b1,
                Cor2R = r2,
                Cor2G = g2,
                Cor2B = b2,
                Modelo = NAPI.Util.VehicleNameToModel(veiculo).ToString(),
                Placa = Functions.GerarPlacaVeiculo(),
                Vida = 1000,
                PosX = concessionaria.PosicaoSpawn.X,
                PosY = concessionaria.PosicaoSpawn.Y,
                PosZ = concessionaria.PosicaoSpawn.Z,
                RotX = concessionaria.RotacaoSpawn.X,
                RotY = concessionaria.RotacaoSpawn.Y,
                RotZ = concessionaria.RotacaoSpawn.Z,
            };

            using (var context = new RoleplayContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            p.Dinheiro -= preco.Valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou {veh.Modelo} por ${preco.Valor:N0}! Use /vspawn {veh.Codigo} para spawnar.");
            NAPI.ClientEvent.TriggerClientEvent(player, "ativarChat");
        }

        [RemoteEvent("gravarContato")]
        public void EVENT_gravarContato(Client player, string nome, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (string.IsNullOrWhiteSpace(nome) || celular == 0)
            {
                Functions.AbrirCelular(player, "Verifique se os campos foram preenchidos corretamente!", 1);
                return;
            }

            if (nome.Length > 25)
            {
                Functions.AbrirCelular(player, "Nome não pode ter mais que 25 caracteres!", 1);
                return;
            }

            var contato = p.Contatos.FirstOrDefault(x => x.Celular == celular);
            if (contato == null)
            {
                p.Contatos.Add(new PersonagemContato()
                {
                    Codigo = p.Codigo,
                    Nome = nome,
                    Celular = celular
                });
                Functions.AbrirCelular(player, $"Contato {celular} adicionado com sucesso!", 2);
            }
            else
            {
                p.Contatos[p.Contatos.IndexOf(contato)].Nome = nome;
                Functions.AbrirCelular(player, $"Contato {celular} editado com sucesso!", 2);
            }
        }

        [RemoteEvent("excluirContato")]
        public void EVENT_excluirContato(Client player, int celular)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            p.Contatos.RemoveAll(x => x.Celular == celular);
            Functions.AbrirCelular(player, $"Celular {celular} removido dos contatos!", 2);
        }

        [RemoteEvent("pagarMulta")]
        public void EVENT_pagarMulta(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            using (var context = new RoleplayContext())
            {
                var multa = context.Multas.FirstOrDefault(x => x.Codigo == codigo);
                if (p.Dinheiro < multa.Valor)
                {
                    Functions.VisualizarMultas(player, "Você não possui dinheiro suficiente!");
                    return;
                }

                multa.DataPagamento = DateTime.Now;
                context.Multas.Update(multa);
                context.SaveChanges();

                p.Dinheiro -= multa.Valor;
                p.SetDinheiro();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pagou a multa {codigo}!");
            NAPI.ClientEvent.TriggerClientEvent(player, "ativarChat");
        }

        [RemoteEvent("comprarConveniencia")]
        public void EVENT_comprarConveniencia(Client player, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var precos = Global.Precos.Where(x => x.Tipo == (int)TipoPreco.Conveniencia).OrderBy(x => x.Nome).Select(x => new
            {
                x.Nome,
                Preco = $"${x.Valor:N0}",
            }).ToList();

            var preco = Global.Precos.FirstOrDefault(x => x.Nome == nome && x.Tipo == (int)TipoPreco.Conveniencia);

            if (p.Dinheiro < preco.Valor)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "comandoComprar", precos, 1, "Você não possui dinheiro suficiente!");
                return;
            }

            string strMensagem = string.Empty;
            switch (nome)
            {
                case "Celular":
                    if (p?.Celular > 0)
                    {
                        NAPI.ClientEvent.TriggerClientEvent(player, "comandoComprar", precos, 1, "Você já possui um celular!");
                        return;
                    }

                    using (var context = new RoleplayContext())
                    {
                        do
                        {
                            p.Celular = new Random().Next(1111111, 9999999);
                            if (p.Celular == 5555555 || context.Personagens.Any(x => x.Celular == p.Celular))
                                p.Celular = 0;

                        } while (p.Celular == 0);
                    }

                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();

                    strMensagem = $"Você comprou um celular! Seu número é: {p.Celular}";
                    break;
                case "Rádio Comunicador":
                    if (p?.CanalRadio > -1)
                    {
                        NAPI.ClientEvent.TriggerClientEvent(player, "comandoComprar", precos, 1, "Você já possui um rádio comunicador!");
                        return;
                    }

                    p.CanalRadio = p.CanalRadio2 = p.CanalRadio3 = 0;
                    p.Dinheiro -= preco.Valor;
                    p.SetDinheiro();

                    strMensagem = $"Você comprou um rádio comunicador!";
                    break;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoComprar", precos, 2, strMensagem);
        }
    }
}